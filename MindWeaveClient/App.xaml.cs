using Microsoft.Extensions.DependencyInjection;
using MindWeaveClient.ChatManagerService;
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.Services.Implementations;
using MindWeaveClient.SocialManagerService;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Utilities.Implementations;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Authentication;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.ViewModel.Authentication;
using MindWeaveClient.ViewModel.Game;
using MindWeaveClient.ViewModel.Main;
using System;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using NavigationService = MindWeaveClient.Utilities.Implementations.NavigationService;

namespace MindWeaveClient
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var langCode = MindWeaveClient.Properties.Settings.Default.languageCode;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langCode);
            base.OnStartup(e);

            var services = new ServiceCollection();
            configureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var audioService = ServiceProvider.GetRequiredService<IAudioService>();
            audioService.initialize();

            var invitationService = ServiceProvider.GetRequiredService<IInvitationService>();
            invitationService.subscribeToGlobalInvites();

            ServiceProvider.GetRequiredService<HeartbeatConnectionHandler>();

            var authWindow = ServiceProvider.GetService<AuthenticationWindow>();
            authWindow.Show();
        }

        private static void configureServices(IServiceCollection services)
        {
            services.AddSingleton<IChatManagerCallback, ChatCallbackHandler>();
            services.AddSingleton<ISocialManagerCallback, SocialCallbackHandler>();

            services.AddSingleton<HeartbeatCallbackHandler>();

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IWindowNavigationService, WindowNavigationService>();
            services.AddSingleton<ICurrentMatchService, CurrentMatchService>();
            services.AddSingleton<IAudioService, AudioService>();
            services.AddSingleton<ICurrentLobbyService, CurrentLobbyService>();

            services.AddSingleton<IHeartbeatService>(provider =>
                new Services.Implementations.HeartbeatService(
                    provider.GetRequiredService<HeartbeatCallbackHandler>()
                )
            );

            services.AddSingleton<IMatchmakingManagerCallback>(provider =>
                new MatchmakingCallbackHandler(
                    provider.GetRequiredService<ICurrentMatchService>(),
                    provider.GetRequiredService<IDialogService>()
                )
            );

            services.AddSingleton<IProfileService, MindWeaveClient.Services.Implementations.ProfileService>();
            services.AddSingleton<IMatchmakingService, MindWeaveClient.Services.Implementations.MatchmakingService>();
            services.AddSingleton<ISocialService, SocialService>();
            services.AddSingleton<IChatService, ChatService>();
            services.AddSingleton<IPuzzleService, PuzzleService>();
            services.AddSingleton<IInvitationService, InvitationService>();

            services.AddSingleton<IAuthenticationService>(provider =>
                new MindWeaveClient.Services.Implementations.AuthenticationService(
                    provider.GetRequiredService<ISocialService>(),
                    provider.GetRequiredService<IHeartbeatService>()
                )
            );

            services.AddSingleton<ISessionCleanupService>(provider =>
                new SessionCleanupService(
                    provider.GetRequiredService<IAuthenticationService>(),
                    provider.GetRequiredService<ISocialService>(),
                    provider.GetRequiredService<IMatchmakingService>(),
                    provider.GetRequiredService<ICurrentMatchService>(),
                    provider.GetRequiredService<IHeartbeatService>()
                )
            );

            services.AddSingleton<IServiceExceptionHandler>(provider =>
                new ServiceExceptionHandler(
                    provider.GetRequiredService<IDialogService>(),
                    provider.GetRequiredService<IWindowNavigationService>(),
                    new Lazy<ISessionCleanupService>(provider.GetRequiredService<ISessionCleanupService>)
                )
            );

            services.AddSingleton<HeartbeatConnectionHandler>(provider =>
                new HeartbeatConnectionHandler(
                    provider.GetRequiredService<IHeartbeatService>(),
                    provider.GetRequiredService<IServiceExceptionHandler>(),
                    new Lazy<ISessionCleanupService>(provider.GetRequiredService<ISessionCleanupService>),
                    provider.GetRequiredService<IDialogService>()
                )
            );

            services.AddTransient<LoginValidator>();
            services.AddTransient<CreateAccountValidator>();
            services.AddTransient<GuestJoinValidator>();
            services.AddTransient<MainMenuValidator>();
            services.AddTransient<PasswordRecoveryValidator>();
            services.AddTransient<VerificationValidator>();
            services.AddTransient<EditProfileValidator>();

            services.AddTransient<LoginViewModel>();
            services.AddTransient<CreateAccountViewModel>();
            services.AddTransient<GuestJoinViewModel>();
            services.AddTransient<PasswordRecoveryViewModel>();
            services.AddTransient<VerificationViewModel>();
            services.AddTransient<MainMenuViewModel>();
            services.AddTransient<LobbyViewModel>();
            services.AddTransient<ProfileViewModel>();
            services.AddTransient<EditProfileViewModel>();
            services.AddTransient<SelectAvatarViewModel>();
            services.AddTransient<SelectionPuzzleViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SocialViewModel>();
            services.AddTransient<GameViewModel>();
            services.AddTransient<PostMatchResultsViewModel>();

            services.AddTransient<LoginPage>();
            services.AddTransient<CreateAccountPage>();
            services.AddTransient<GuestJoinPage>();
            services.AddTransient<PasswordRecoveryPage>();
            services.AddTransient<VerificationPage>();
            services.AddTransient<MainMenuPage>();
            services.AddTransient<LobbyPage>();
            services.AddTransient<ProfilePage>();
            services.AddTransient<EditProfilePage>();
            services.AddTransient<SelectAvatarPage>();
            services.AddTransient<SelectionPuzzlePage>();
            services.AddTransient<SocialPage>();
            services.AddTransient<GamePage>();
            services.AddTransient<PostMatchResultsPage>();

            services.AddTransient<AuthenticationWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<SettingsWindow>();
            services.AddTransient<GameWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                cleanupHeartbeatService();

                cleanupCallbackHandlers();
                cleanupServices();
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                base.OnExit(e);
            }
        }

        private static void cleanupHeartbeatService()
        {
            try
            {
                var heartbeatService = ServiceProvider.GetService<IHeartbeatService>();
                heartbeatService?.forceStop();
                heartbeatService?.Dispose();

                var heartbeatHandler = ServiceProvider.GetService<HeartbeatConnectionHandler>();
                heartbeatHandler?.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void cleanupCallbackHandlers()
        {
            try
            {
                MatchmakingCallbackHandler.clearAllHandlers();

            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void cleanupServices()
        {
            var audioService = ServiceProvider.GetService<IAudioService>();
            audioService?.Dispose();

            var invitationService = ServiceProvider.GetService<IInvitationService>();
            invitationService?.unsubscribeFromGlobalInvites();

            cleanupSocialService();

            var matchmakingService = ServiceProvider.GetService<IMatchmakingService>();
            matchmakingService?.disconnect();

            var chatService = ServiceProvider.GetService<IChatService>();
            if (chatService is IDisposable chatDisposable)
            {
                chatDisposable.Dispose();
            }
        }

        private static void cleanupSocialService()
        {
            var socialService = ServiceProvider.GetService<ISocialService>();
            if (socialService == null) return;

            try
            {
                var username = SessionService.Username;
                if (!string.IsNullOrEmpty(username))
                {
                    socialService.disconnectAsync(username).Wait(TimeSpan.FromSeconds(2));
                }
            }
            catch (CommunicationException)
            {
                //ignored
            }
            catch (TimeoutException)
            {
                //ignored
            }
            catch (AggregateException)
            {
                //ignored
            }
            finally
            {
                socialService.Dispose();
            }
        }
    }
}