using MindWeaveClient.Helpers;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Utilities.Implementations;
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
        private const int MAX_LOBBY_CODE_LENGTH = 6;

        private readonly IMatchmakingService matchmakingService;
        private readonly ISessionCleanupService cleanupService;
        private readonly ICurrentLobbyService currentLobbyService;
        private readonly IDialogService dialogService;
        private readonly MainMenuValidator validator;
        private readonly IWindowNavigationService windowNavigationService;
        private readonly IServiceExceptionHandler exceptionHandler;
        private bool isDisposed;

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
                string normalizedValue = value?.Trim();
                string processedValue = clampString(normalizedValue, MAX_LOBBY_CODE_LENGTH);

                if (joinLobbyCode != processedValue)
                {
                    joinLobbyCode = processedValue;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(processedValue))
                    {
                        markAsTouched(nameof(JoinLobbyCode));
                    }

                    validate(validator, this, "JoinLobby");
                    OnPropertyChanged(nameof(JoinLobbyCodeError));
                }
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S107:Methods should not have too many parameters",
            Justification = "Dependencies are injected via DI container - this is standard practice for ViewModels")]
        public MainMenuViewModel(
            IMatchmakingService matchmakingService,
            MainMenuValidator validator,
            INavigationService navigationService,
            IWindowNavigationService windowNavigationService,
            ISessionCleanupService cleanupService,
            ICurrentLobbyService currentLobbyService,
            IDialogService dialogService,
            IServiceExceptionHandler exceptionHandler)
        {
            this.matchmakingService = matchmakingService;
            this.validator = validator;
            var navigationService1 = navigationService;
            this.windowNavigationService = windowNavigationService;
            this.cleanupService = cleanupService;
            this.currentLobbyService = currentLobbyService;
            this.dialogService = dialogService;
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
            ExitCommand = new RelayCommand(p => executeExit());

            validate(validator, this, "JoinLobby");
        }

        private static string clampString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
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
                var result = await matchmakingService.joinLobbyWithConfirmationAsync(
                    SessionService.Username,
                    JoinLobbyCode);

                if (result.Success)
                {
                    currentLobbyService.setInitialState(result.InitialLobbyState);
                    windowNavigationService.openWindow<GameWindow>();
                    markWindowAsSafeClose();
                    windowNavigationService.closeWindow<MainWindow>();
                }
                else
                {
                    string localizedMessage = MessageCodeInterpreter.translate(
                        result.MessageCode,
                        Lang.ErrorGeneric);
                    dialogService.showWarning(localizedMessage, Lang.WarningTitle);
                    disconnectMatchmakingSafe();
                }
            }
            catch (Exception ex)
            {
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

        private static void executeExit()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.Close();
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
                //ignored
            }
            catch (ObjectDisposedException)
            {
                //ignored
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                SessionService.AvatarPathChanged -= OnAvatarPathChanged;
            }

            isDisposed = true;
        }

    }
}