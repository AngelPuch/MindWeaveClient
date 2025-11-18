using System;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.ViewModel.Game;
using System.Diagnostics;
using System.Windows;

namespace MindWeaveClient.View.Game
{
    public partial class GameWindow : Window
    {
        private readonly INavigationService navigationService;
        private bool isHandlingClosing;

        public GameWindow(INavigationService navigationService, LobbyPage startPage)
        {
            InitializeComponent();

            this.navigationService = navigationService;
            this.navigationService.initialize(GameFrame);
            GameFrame.Content = startPage;

            this.Closing += GameWindow_Closing;
        }

        private async void GameWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isHandlingClosing)
            {
                return;
            }

            e.Cancel = true;
            isHandlingClosing = true;

            try
            {
                var lobbyPage = GameFrame?.Content as LobbyPage;
                if (lobbyPage?.DataContext is LobbyViewModel lobbyViewModel)
                {
                    await lobbyViewModel.cleanup();
                }

            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error during graceful cleanup: {ex.Message}");
            }
            finally
            {
                this.Closing -= GameWindow_Closing;
            }

        }
    }
}