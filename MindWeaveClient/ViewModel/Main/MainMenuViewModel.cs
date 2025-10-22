// MindWeaveClient/ViewModel/Main/MainMenuViewModel.cs

using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MindWeaveClient.ViewModel.Main
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly Action<Page> navigateTo;
        private readonly Page mainMenuPage;

        public string playerUsername { get; }
        public string playerAvatarPath { get; }

        public ICommand profileCommand { get; }
        public ICommand playCommand { get; }
        public ICommand socialCommand { get; }
        public ICommand settingsCommand { get; }

        private bool isBusyValue;
        public bool isBusy
        {
            get => isBusyValue;
            set { isBusyValue = value; OnPropertyChanged(); ((RelayCommand)playCommand).RaiseCanExecuteChanged(); /* Actualiza CanExecute */ }
        }
        private MatchmakingManagerClient matchmakingProxy => MatchmakingServiceClientManager.Instance.Proxy;
        public MainMenuViewModel(Action<Page> navigateTo, Page mainMenuPage)
        {
            this.navigateTo = navigateTo;
            this.mainMenuPage = mainMenuPage;

            playerUsername = SessionService.username;
            playerAvatarPath = SessionService.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

            profileCommand = new RelayCommand(p => executeGoToProfile());
            socialCommand = new RelayCommand(p => executeGoToSocial());
            settingsCommand = new RelayCommand(p => { /* TODO: Implementar */ MessageBox.Show("Settings not implemented yet."); });
            // TO-DO: Implementar otros comandos
        }

        private async Task executePlayAsync()
        {
            isBusy = true;
            try
            {
                // 1. Asegurar la conexión usando el gestor
                if (!MatchmakingServiceClientManager.Instance.EnsureConnected())
                {
                    MessageBox.Show("Could not connect to the matchmaking service.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO: Lang
                    return; // Salir si no se puede conectar
                }

                // 2. Definir configuraciones (igual que antes)
                var defaultSettings = new LobbySettingsDto
                {
                    difficultyId = 1,
                    preloadedPuzzleId = 1
                };

                // 3. Llamar al método del servicio WCF usando el proxy del gestor
                LobbyCreationResultDto result = await matchmakingProxy.createLobbyAsync(SessionService.username, defaultSettings);

                // 4. Procesar resultado (igual que antes)
                if (result.success)
                {
                    MessageBox.Show($"Lobby created! Code: {result.lobbyCode}", "Success", MessageBoxButton.OK, MessageBoxImage.Information); // TODO: Lang

                    // Navegar a LobbyPage
                    var lobbyPage = new LobbyPage();
                    // TODO: Crear LobbyViewModel y pasar result.initialLobbyState
                    // lobbyPage.DataContext = new LobbyViewModel(result.initialLobbyState, navigateTo); // Pasar acción de navegación si es necesario
                    navigateTo(lobbyPage);
                }
                else
                {
                    MessageBox.Show($"Failed to create lobby: {result.message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); // TODO: Lang
                }
                // NOTA: No cerramos el proxy aquí, el gestor maneja su ciclo de vida.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); // TODO: Lang
                // Considerar desconectar/reconectar en caso de error grave
                MatchmakingServiceClientManager.Instance.Disconnect();
            }
            finally
            {
                isBusy = false;
            }
        }

        private void executeGoToProfile()
        {
            var profilePage = new ProfilePage();
            // Le pasamos al ProfileViewModel las dos instrucciones que necesita
            profilePage.DataContext = new ProfileViewModel(
                () => navigateTo(mainMenuPage),      // 1. Para volver al menú
                () => executeGoToEditProfile()       // 2. Para ir a editar
            );
            navigateTo(profilePage);
        }

        private void executeGoToSocial() {
            navigateTo(new SocialPage());
        }

        private void executeGoToEditProfile()
        {
            var editProfilePage = new EditProfilePage();
            // Ahora le pasamos al EditProfileViewModel las DOS instrucciones que pide
            editProfilePage.DataContext = new EditProfileViewModel(
                () => executeGoToProfile(),          // 1. Para cancelar y volver al perfil
                () => executeGoToSelectAvatar()      // 2. Para cambiar el avatar
            );
            navigateTo(editProfilePage);
        }

        private void executeGoToSelectAvatar()
        {
            var selectAvatarPage = new SelectAvatarPage(); // <-- Aún no hemos creado este archivo XAML
                                                           // Creamos el ViewModel de la galería y le pasamos la instrucción para volver a la página de edición
            selectAvatarPage.DataContext = new SelectAvatarViewModel(
                () => executeGoToEditProfile()
            );
            navigateTo(selectAvatarPage);
        }

    }
}