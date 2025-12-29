using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Authentication;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class MainMenuViewModel : BaseViewModel, IDisposable
    {
        private readonly IMatchmakingService matchmakingService;
        private readonly ISessionCleanupService cleanupService;
        private readonly MainMenuValidator validator;
        private readonly IWindowNavigationService windowNavigationService;
        private readonly IServiceExceptionHandler exceptionHandler;

        public string PlayerUsername { get; }

        private string playerAvatarPath;
        public string PlayerAvatarPath
        {
            get => playerAvatarPath;
            set
            {
                playerAvatarPath = value;
                OnPropertyChanged();
            }
        }

        private string joinLobbyCode = string.Empty;
        public string JoinLobbyCode
        {
            get => joinLobbyCode;
            set
            {
                joinLobbyCode = value?.Trim() ?? string.Empty;
                OnPropertyChanged();

                if (!string.IsNullOrEmpty(joinLobbyCode))
                {
                    markAsTouched(nameof(JoinLobbyCode));
                }

                validate(validator, this, "JoinLobby");
                OnPropertyChanged(nameof(JoinLobbyCodeError));
            }
        }

        public string JoinLobbyCodeError
        {
            get
            {
                var errors = GetErrors(nameof(JoinLobbyCode)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public ICommand ProfileCommand { get; }
        public ICommand CreateLobbyCommand { get; }
        public ICommand SocialCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand JoinLobbyCommand { get; }
        public ICommand LogOutCommand { get; }
        public ICommand ExitCommand { get; }

        public MainMenuViewModel(
            IMatchmakingService matchmakingService,
            MainMenuValidator validator,
            INavigationService navigationService,
            IWindowNavigationService windowNavigationService,
            ISessionCleanupService cleanupService,
            IServiceExceptionHandler exceptionHandler)
        {
            this.matchmakingService = matchmakingService;
            this.validator = validator;
            var navigationService1 = navigationService;
            this.windowNavigationService = windowNavigationService;
            this.cleanupService = cleanupService;
            this.exceptionHandler = exceptionHandler;

            SessionService.AvatarPathChanged += OnAvatarPathChanged;

            PlayerUsername = SessionService.Username;
            PlayerAvatarPath = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

            ProfileCommand = new RelayCommand(p => 
                navigationService1.navigateTo<ProfilePage>(), p => !IsBusy);
            CreateLobbyCommand = new RelayCommand(p => 
                navigationService1.navigateTo<SelectionPuzzlePage>(), p => !IsBusy);
            SocialCommand = new RelayCommand(p => 
                navigationService1.navigateTo<SocialPage>(), p => !IsBusy);
            SettingsCommand = new RelayCommand(p => 
                this.windowNavigationService.openDialog<SettingsWindow>(Application.Current.MainWindow), p => !IsBusy);
            JoinLobbyCommand = new RelayCommand(async p => 
                await executeJoinLobbyAsync(), p => !HasErrors && !IsBusy);
            LogOutCommand = new RelayCommand(async p => await executeLogOutAsync(), p => !IsBusy);
            ExitCommand = new RelayCommand(async p => await executeExitAsync());

            validate(validator, this, "JoinLobby");
        }

        private void OnAvatarPathChanged(object sender, EventArgs e)
        {
            PlayerAvatarPath = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
        }

        private async Task executeJoinLobbyAsync()
        {
            markAsTouched(nameof(JoinLobbyCode));

            if (HasErrors) return;

            SetBusy(true);
            try
            {
                await matchmakingService.joinLobbyAsync(SessionService.Username, JoinLobbyCode);
                windowNavigationService.openWindow<GameWindow>();
                markWindowAsSafeClose();
                windowNavigationService.closeWindow<MainWindow>();
            }
            catch (Exception ex)
            {
                SetBusy(false);
                exceptionHandler.handleException(ex, Lang.JoinLobbyOperation);
                disconnectMatchmakingSafe();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeLogOutAsync()
        {
            SetBusy(true);
            try
            {
                await cleanupService.cleanUpSessionAsync();

                windowNavigationService.openWindow<AuthenticationWindow>();
                markWindowAsSafeClose();
                windowNavigationService.closeWindow<MainWindow>();
            }
            catch (EndpointNotFoundException)
            {
                forceLocalLogout();
            }
            catch (CommunicationException)
            {
                forceLocalLogout();
            }
            catch (TimeoutException)
            {
                forceLocalLogout();
            }
            catch (SocketException)
            {
                forceLocalLogout();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeExitAsync()
        {
            SetBusy(true);
            try
            {
                await cleanupService.cleanUpSessionAsync();
            }
            catch (EndpointNotFoundException)
            {
            }
            catch (CommunicationException)
            {
            }
            catch (TimeoutException)
            {
            }
            catch (SocketException)
            {
            }
            finally
            {
                SetBusy(false);
                Application.Current.Shutdown();
            }

        }

        private void forceLocalLogout()
        {
            SessionService.clearSession();
            windowNavigationService.openWindow<AuthenticationWindow>();
            markWindowAsSafeClose();
            windowNavigationService.closeWindow<MainWindow>();
        }

        private static void markWindowAsSafeClose()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.IsExitConfirmed = true;
            }
        }

        private void disconnectMatchmakingSafe()
        {
            try
            {
                matchmakingService.disconnect();
            }
            catch (CommunicationException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void Dispose()
        {
            SessionService.AvatarPathChanged -= OnAvatarPathChanged;
        }
    }
}