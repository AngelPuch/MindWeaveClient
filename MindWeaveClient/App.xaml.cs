// MindWeaveClient/App.xaml.cs
using System;
using System.Linq; // Needed for OfType<T>()
using System.Threading;
using System.Windows;
using MindWeaveClient.Services;
using MindWeaveClient.View.Game;
using MindWeaveClient.ViewModel.Game;
using System.Windows.Navigation;
using MindWeaveClient.View.Main;
using MindWeaveClient.Properties.Langs;
using System.Diagnostics;

namespace MindWeaveClient
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var langCode = MindWeaveClient.Properties.Settings.Default.languageCode;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langCode);
            base.OnStartup(e);

            // Subscription happens in MainWindow.Loaded
            Debug.WriteLine("App.OnStartup completed.");
        }

        // --- SubscribeToGlobalInvites (No changes needed here) ---
        public static void SubscribeToGlobalInvites()
        {
            if (SocialServiceClientManager.Instance.Proxy?.State == System.ServiceModel.CommunicationState.Opened &&
                SocialServiceClientManager.Instance.CallbackHandler != null)
            {
                SocialServiceClientManager.Instance.CallbackHandler.LobbyInviteReceived -= App_LobbyInviteReceived;
                SocialServiceClientManager.Instance.CallbackHandler.LobbyInviteReceived += App_LobbyInviteReceived;
                Debug.WriteLine("App.xaml.cs: Subscribed to global LobbyInviteReceived event from Social Service.");
            }
            else
            {
                Debug.WriteLine($"App.xaml.cs: ERROR - Cannot subscribe to invites. Social Service Proxy State: {SocialServiceClientManager.Instance.Proxy?.State}, CallbackHandler Null: {SocialServiceClientManager.Instance.CallbackHandler == null}");
                if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsLoaded)
                {
                    MessageBox.Show("Could not initialize invitation system. Please restart.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // --- App_LobbyInviteReceived (No changes needed here) ---
        private static void App_LobbyInviteReceived(string fromUsername, string lobbyId)
        {
            Debug.WriteLine($"App_LobbyInviteReceived: Invite from {fromUsername} for lobby {lobbyId}. Current user: {SessionService.username}");

            if (fromUsername.Equals(SessionService.username, StringComparison.OrdinalIgnoreCase))
            { Debug.WriteLine("App_LobbyInviteReceived: Ignored self-invite."); return; }

            MessageBoxResult result = MessageBox.Show(
                $"You received a lobby invitation from {fromUsername}.\nLobby Code: {lobbyId}\n\nDo you want to join?",
                "Lobby Invitation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Debug.WriteLine($"User {SessionService.username} accepted invite to lobby {lobbyId}. Ensuring Matchmaking connection...");
                if (MatchmakingServiceClientManager.Instance.EnsureConnected())
                {
                    Debug.WriteLine("Matchmaking service connected. Calling joinLobby...");
                    try
                    {
                        var matchmakingProxy = MatchmakingServiceClientManager.Instance.Proxy;
                        if (string.IsNullOrEmpty(SessionService.username))
                        {
                            Debug.WriteLine("ERROR: SessionService.username is null when trying to join lobby.");
                            MessageBox.Show("Session error. Cannot join lobby.", Lang.ErrorTitle);
                            return;
                        }
                        matchmakingProxy.joinLobby(SessionService.username, lobbyId);

                        Debug.WriteLine($"Navigating to LobbyPage for lobby {lobbyId}...");
                        NavigateToLobbyPage(); // Call the corrected helper
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Exception during joinLobby or navigation: {ex}");
                        MessageBox.Show($"Error trying to join lobby {lobbyId}: {ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        MatchmakingServiceClientManager.Instance.Disconnect();
                    }
                }
                else
                {
                    Debug.WriteLine("Failed to connect to Matchmaking service.");
                    MessageBox.Show(Lang.CannotConnectMatchmaking, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                Debug.WriteLine($"User {SessionService.username} declined invite to lobby {lobbyId}.");
            }
        }

        // *** HELPER PARA NAVEGACIÓN (CORREGIDO) ***
        private static void NavigateToLobbyPage()
        {
            // Ejecutar en el hilo de UI
            Application.Current.Dispatcher.Invoke(() => {
                // *** CAMBIO: Buscar la MainWindow en la colección de ventanas abiertas ***
                MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

                // Verificar que encontramos la MainWindow y que su MainFrame es usable
                if (mainWindow != null && mainWindow.MainFrame?.NavigationService != null)
                {
                    Debug.WriteLine("NavigateToLobbyPage: Found MainWindow and MainFrame. Proceeding with navigation.");

                    // Limpiar historial de navegación (opcional pero recomendado)
                    while (mainWindow.MainFrame.NavigationService.CanGoBack)
                    {
                        mainWindow.MainFrame.NavigationService.RemoveBackEntry();
                    }

                    // Crear la página y el ViewModel
                    var lobbyPage = new LobbyPage();
                    lobbyPage.DataContext = new LobbyViewModel(
                        null, // Estado inicial nulo al unirse
                        page => mainWindow.MainFrame.Navigate(page), // Navegar adelante
                                                                     // Navegar atrás (volver al menú principal)
                        () => mainWindow.MainFrame.Navigate(new MainMenuPage(page => mainWindow.MainFrame.Navigate(page)))
                    );

                    // Realizar la navegación
                    mainWindow.MainFrame.Navigate(lobbyPage);
                    Debug.WriteLine("Successfully navigated to LobbyPage.");
                }
                else // Error si no se encontró MainWindow o MainFrame
                {
                    // Log detallado del error
                    string errorReason = "Unknown reason";
                    if (mainWindow == null)
                    {
                        errorReason = "Could not find an open window of type MainWindow in Application.Current.Windows.";
                        // Log adicional: ¿Qué ventanas están abiertas?
                        var openWindowTypes = string.Join(", ", Application.Current.Windows.OfType<Window>().Select(w => w.GetType().Name));
                        Debug.WriteLine($"Open windows: [{openWindowTypes}]");
                    }
                    else if (mainWindow.MainFrame == null) { errorReason = "Found MainWindow, but its MainFrame property is null."; }
                    else if (mainWindow.MainFrame.NavigationService == null) { errorReason = "Found MainWindow and MainFrame, but MainFrame.NavigationService is null."; }

                    Debug.WriteLine($"Navigation Error: Could not find MainWindow or MainFrame for navigation. Reason: {errorReason}");
                    MessageBox.Show($"Navigation Error: Could not find the main navigation frame.\n({errorReason})", Lang.ErrorTitle);
                }
            });
        }


        protected override void OnExit(ExitEventArgs e)
        {
            // Desuscripción (sin cambios)
            if (SocialServiceClientManager.Instance.CallbackHandler != null)
            {
                SocialServiceClientManager.Instance.CallbackHandler.LobbyInviteReceived -= App_LobbyInviteReceived;
                Debug.WriteLine("App.xaml.cs: Unsubscribed from global LobbyInviteReceived event.");
            }

            // Desconectar servicios (sin cambios)
            SocialServiceClientManager.Instance.Disconnect();
            MatchmakingServiceClientManager.Instance.Disconnect();

            base.OnExit(e);
        }
    }
}