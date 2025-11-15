using Microsoft.Extensions.DependencyInjection;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Implementations;
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
using System.Threading;
using System.Windows;
using NavigationService = MindWeaveClient.Utilities.Implementations.NavigationService;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.SocialManagerService;
using MindWeaveClient.ChatManagerService;

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

            var authWindow = ServiceProvider.GetService<AuthenticationWindow>();
            authWindow.Show();
        }

        private void configureServices(IServiceCollection services)
        {
            services.AddSingleton<IChatManagerCallback, ChatCallbackHandler>();
            services.AddSingleton<ISocialManagerCallback, SocialCallbackHandler>();
            services.AddSingleton<IMatchmakingManagerCallback>(provider =>
                new MatchmakingCallbackHandler(
                    provider.GetRequiredService<ICurrentMatchService>(),
                    provider.GetRequiredService<IDialogService>()
                )
            );

            services.AddSingleton<IAuthenticationService, MindWeaveClient.Services.Implementations.AuthenticationService>();
            services.AddSingleton<IProfileService, MindWeaveClient.Services.Implementations.ProfileService>();
            services.AddSingleton<IMatchmakingService, MindWeaveClient.Services.Implementations.MatchmakingService>();
            services.AddSingleton<ISocialService, SocialService>();
            services.AddSingleton<IChatService, ChatService>();
            services.AddSingleton<IPuzzleService, PuzzleService>();
            services.AddSingleton<IInvitationService, InvitationService>();

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IWindowNavigationService, WindowNavigationService>();
            services.AddSingleton<ICurrentMatchService, CurrentMatchService>();
            services.AddSingleton<IAudioService, AudioService>();
            services.AddSingleton<ICurrentLobbyService, CurrentLobbyService>();

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

            services.AddTransient<AuthenticationWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<SettingsWindow>();
            services.AddTransient<GameWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceProvider.GetService<IAudioService>()?.Dispose();
            ServiceProvider.GetService<IInvitationService>()?.unsubscribeFromGlobalInvites();

            var socialService = ServiceProvider.GetService<ISocialService>();
            if (socialService is IDisposable socialDisposable)
            {
                socialDisposable.Dispose();
            }

            ServiceProvider.GetService<IMatchmakingService>()?.disconnect();

            var chatService = ServiceProvider.GetService<IChatService>();
            if (chatService is IDisposable chatDisposable)
            {
                chatDisposable.Dispose();
            }
            base.OnExit(e);
        }
    }
}