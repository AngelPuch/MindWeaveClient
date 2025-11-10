using Microsoft.Extensions.DependencyInjection;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Implementations;
using MindWeaveClient.Utilities;
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
using MindWeaveClient.Services;
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
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var audioService = ServiceProvider.GetRequiredService<IAudioService>();
            audioService.initialize();

            // Suscribir a invitaciones globales a través del servicio
            var invitationService = ServiceProvider.GetRequiredService<IInvitationService>();
            invitationService.subscribeToGlobalInvites();

            var authWindow = ServiceProvider.GetService<AuthenticationWindow>();
            authWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Servicios WCF
            services.AddSingleton<IAuthenticationService, MindWeaveClient.Services.Implementations.AuthenticationService>();
            services.AddSingleton<IProfileService, MindWeaveClient.Services.Implementations.ProfileService>();
            services.AddSingleton<IMatchmakingService, MindWeaveClient.Services.Implementations.MatchmakingService>();
            services.AddSingleton<ISocialService, MindWeaveClient.Services.Implementations.SocialService>();
            services.AddSingleton<IChatService, MindWeaveClient.Services.Implementations.ChatService>();
            services.AddSingleton<IPuzzleService, Services.Implementations.PuzzleService>();
            services.AddSingleton<IInvitationService, InvitationService>();

            // Servicios de utilidades
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IWindowNavigationService, WindowNavigationService>();
            services.AddSingleton<ICurrentLobbyService, CurrentLobbyService>();
            services.AddSingleton<ICurrentMatchService, CurrentMatchService>();
            services.AddSingleton<IAudioService, AudioService>();

            // Validadores
            services.AddTransient<LoginValidator>();
            services.AddTransient<CreateAccountValidator>();
            services.AddTransient<GuestJoinValidator>();
            services.AddTransient<MainMenuValidator>();
            services.AddTransient<PasswordRecoveryValidator>();
            services.AddTransient<VerificationValidator>();
            services.AddTransient<EditProfileValidator>();

            // ViewModels
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

            // Vistas (Pages)
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

            // Ventanas
            services.AddTransient<AuthenticationWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<SettingsWindow>();
            services.AddTransient<GameWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Detener audio
            ServiceProvider.GetService<IAudioService>()?.Dispose();

            // Desuscribir de invitaciones globales
            ServiceProvider.GetService<IInvitationService>()?.unsubscribeFromGlobalInvites();

            // Desconectar servicios
            var socialService = ServiceProvider.GetService<ISocialService>();
            if (socialService != null)
            {
                try
                {
                    socialService.disconnectAsync(SessionService.Username).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError($"Error disconnecting social service: {ex.Message}");
                }
            }

            ServiceProvider.GetService<IMatchmakingService>()?.disconnect();

            var chatService = ServiceProvider.GetService<IChatService>();
            if (chatService != null)
            {
                try
                {
                    chatService.disconnectAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError($"Error disconnecting chat service: {ex.Message}");
                }
            }

            base.OnExit(e);
        }
    }
}