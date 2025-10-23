// MindWeaveClient/View/Main/MainWindow.xaml.cs
using MindWeaveClient.View.Game;
using System.Windows;
// *** AÑADIR Usings ***
using MindWeaveClient;      // Para App
using MindWeaveClient.Services; // Para SocialServiceClientManager
using System;               // Para Console.WriteLine
using System.Diagnostics;   // Para Debug.WriteLine

namespace MindWeaveClient.View.Main
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Navega a la página inicial (MainMenuPage)
            MainFrame.Navigate(new MainMenuPage(page => MainFrame.Navigate(page)));

            // *** SUSCRIBIRSE A INVITACIONES CUANDO LA VENTANA ESTÉ CARGADA ***
            this.Loaded += MainWindow_Loaded;
            // Opcional: Desuscribirse cuando se cierra para limpiar
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MainWindow_Loaded: Attempting to subscribe to global invites...");
            // Llama al método estático de App.xaml.cs para realizar la suscripción
            // Esto asume que el SocialService ya está conectado desde el Login
            App.SubscribeToGlobalInvites();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // Opcional pero buena práctica: Desuscribirse
            if (SocialServiceClientManager.Instance.CallbackHandler != null)
            {
                // Necesitamos una referencia al método estático. Guardémosla o usemos el nombre completo.
                // SocialServiceClientManager.Instance.CallbackHandler.LobbyInviteReceived -= App.App_LobbyInviteReceived; // No funciona directamente
                // Mejor manejar la desuscripción globalmente en App.OnExit
                Debug.WriteLine("MainWindow_Closed: Global invite handler cleanup is managed by App.OnExit.");
            }

            // Desconectar servicios (esto ya debería estar en App.OnExit, pero podemos ser redundantes)
            // SocialServiceClientManager.Instance.Disconnect();
            // MatchmakingServiceClientManager.Instance.Disconnect();
        }
    }
}