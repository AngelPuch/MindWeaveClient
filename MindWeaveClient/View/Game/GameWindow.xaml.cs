using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.ViewModel.Game;
using System.Diagnostics;
using System.Windows;

namespace MindWeaveClient.View.Game
{
    public partial class GameWindow : Window
    {
        private readonly INavigationService navigationService;

        public GameWindow(INavigationService navigationService, LobbyPage startPage)
        {
            InitializeComponent();

            this.navigationService = navigationService;
            this.navigationService.initialize(GameFrame);
            GameFrame.Content = startPage;

            // Suscribirse al evento Closing para limpiar recursos
            this.Closing += GameWindow_Closing;

            Trace.TraceInformation("GameWindow initialized");
        }

        private void GameWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Trace.TraceInformation("GameWindow closing - cleaning up lobby resources");

            // Si hay una LobbyPage activa, limpiar su ViewModel
            var lobbyPage = GameFrame?.Content as LobbyPage;
            if (lobbyPage?.DataContext is LobbyViewModel lobbyViewModel)
            {
                // No cancelar el cierre, pero hacer cleanup
                lobbyViewModel.cleanup();
                Trace.TraceInformation("LobbyViewModel cleanup completed from window closing");
            }

            // Si hay otras páginas con recursos que limpiar, hacerlo aquí

            this.Closing -= GameWindow_Closing;
        }
    }
}