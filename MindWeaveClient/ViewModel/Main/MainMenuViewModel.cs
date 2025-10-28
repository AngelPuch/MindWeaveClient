using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.View.Settings;
using MindWeaveClient.ViewModel.Game;
using System;
using System.Linq; // Necesario para OfType<MainWindow>()
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly Action<Page> navigateTo;
        private readonly Page mainMenuPage;

        public string playerUsername { get; }
        public string playerAvatarPath { get; } // Verifica este binding en XAML

        // --- Comandos ---
        public ICommand profileCommand { get; }
        // *** RENOMBRADO AQUÍ ***
        public ICommand createLobbyCommand { get; } // Antes era playCommand
        public ICommand socialCommand { get; }
        public ICommand settingsCommand { get; }
        public ICommand joinLobbyCommand { get; } // Verifica este binding en XAML

        // --- Propiedades Join Lobby ---
        private string joinLobbyCodeValue = string.Empty;
        public string joinLobbyCode // Verifica este binding en XAML
        {
            get => joinLobbyCodeValue;
            set
            {
                joinLobbyCodeValue = value?.ToUpper().Trim() ?? string.Empty;
                if (joinLobbyCodeValue.Length > 6) { joinLobbyCodeValue = joinLobbyCodeValue.Substring(0, 6); }
                OnPropertyChanged();
                OnPropertyChanged(nameof(canJoinLobby));
                ((RelayCommand)joinLobbyCommand).RaiseCanExecuteChanged();
            }
        }
        public bool canJoinLobby => !isBusy && !string.IsNullOrWhiteSpace(joinLobbyCode) && joinLobbyCode.Length == 6; // Verifica este binding en XAML

        // --- Estado Busy ---
        private bool isBusyValue;
        public bool isBusy
        {
            get => isBusyValue;
            set
            {
                isBusyValue = value;
                OnPropertyChanged();
                // Actualizar CanExecute de AMBOS comandos
                ((RelayCommand)createLobbyCommand).RaiseCanExecuteChanged(); // Usa el nuevo nombre
                ((RelayCommand)joinLobbyCommand).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(canJoinLobby));
            }
        }

        // Proxy (sin cambios)
        private MatchmakingManagerClient matchmakingProxy => MatchmakingServiceClientManager.Instance.Proxy;

        public MainMenuViewModel(Action<Page> navigateTo, Page mainMenuPage)
        {
            this.navigateTo = navigateTo;
            this.mainMenuPage = mainMenuPage;

            playerUsername = SessionService.username;
            // Asegúrate que SessionService.avatarPath tenga un valor o el default
            playerAvatarPath = SessionService.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

            profileCommand = new RelayCommand(p => executeGoToProfile());
            // *** USA EL NUEVO NOMBRE AQUÍ ***
            createLobbyCommand = new RelayCommand(p => executeGoToPuzzleSelection(), p => !isBusy); // Llama al método correcto
            socialCommand = new RelayCommand(p => executeGoToSocial());
            settingsCommand = new RelayCommand(p => executeShowSettings(), p => !isBusy);
            joinLobbyCommand = new RelayCommand(async p => await executeJoinLobbyAsync(), p => canJoinLobby);

            // Log inicial para verificar el avatarPath
            Console.WriteLine($"MainMenuViewModel Initialized. Avatar Path: {playerAvatarPath}");
        }
        private void executeGoToPuzzleSelection()
        {
            // Navega a la página de selección de puzzles
            var selectionPage = new SelectionPuzzlePage(); // Asegúrate que el using esté arriba
            selectionPage.DataContext = new SelectionPuzzleViewModel(
                navigateTo,             // Acción para navegar hacia adelante (al Lobby)
                () => navigateTo(mainMenuPage) // Acción para cancelar (volver al menú)
            );
            navigateTo(selectionPage);
        }
        private async Task executeJoinLobbyAsync()
        {
            if (!canJoinLobby) return;
            if (!Regex.IsMatch(joinLobbyCode, "^[A-Z0-9]{6}$")) { /*...*/ return; }

            isBusy = true;
            try
            {
                if (!MatchmakingServiceClientManager.Instance.EnsureConnected()) { /*...*/ return; }

                matchmakingProxy.joinLobby(SessionService.username, joinLobbyCode);
                MessageBox.Show($"Attempting to join lobby {joinLobbyCode}...", "Joining", MessageBoxButton.OK, MessageBoxImage.Information); // Opcional

                var lobbyPage = new LobbyPage();
                lobbyPage.DataContext = new LobbyViewModel(
                    null, // Estado inicial nulo al unirse
                    navigateTo,
                    () => navigateTo(mainMenuPage)
                );
                navigateTo(lobbyPage);
            }
            catch (Exception ex) // Captura general, considera específicas
            {
                MessageBox.Show($"An error occurred while joining lobby: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); // TODO: Lang
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
            var selectAvatarPage = new SelectAvatarPage();
                                                           // Creamos el ViewModel de la galería y le pasamos la instrucción para volver a la página de edición
            selectAvatarPage.DataContext = new SelectAvatarViewModel(
                () => executeGoToEditProfile()
            );
            navigateTo(selectAvatarPage);
        }
        private void executeShowSettings()
        {
            var settingsWindow = new SettingsWindow();
            // Muestra la ventana como un diálogo modal (bloquea la ventana principal)
            settingsWindow.Owner = Application.Current.MainWindow; // Establece la ventana principal como dueña
            bool? result = settingsWindow.ShowDialog();

            // 'result' será true si se guardó, false o null si se canceló.
            // Puedes actuar según el resultado si es necesario (ej. refrescar algo)
            if (result == true)
            {
                // Settings were saved
                Console.WriteLine("Settings saved.");
            }
            else
            {
                // Settings were canceled
                Console.WriteLine("Settings canceled.");
            }
        }

    }
}