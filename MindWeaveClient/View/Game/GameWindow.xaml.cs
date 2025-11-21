using System;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.ViewModel.Game;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.View.Game
{
    public partial class GameWindow : Window
    {
        private bool isHandlingClosing;

        public GameWindow(INavigationService navigationService, LobbyPage startPage)
        {
            InitializeComponent();

            navigationService.initialize(GameFrame);
            GameFrame.Content = startPage;

            this.Closing += gameWindowClosing;
        }

        private async void gameWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isHandlingClosing) { return; }

            e.Cancel = true;
            isHandlingClosing = true;

            try
            {
                var currentPage = GameFrame?.Content;

                if (currentPage is LobbyPage lobbyPage && lobbyPage.DataContext is LobbyViewModel lobbyVm)
                {
                    await lobbyVm.cleanup();
                }
                else if (currentPage is Page page && page.DataContext is IDisposable disposableVm)
                {
                    disposableVm.Dispose();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error during graceful cleanup: {ex.Message}");
            }
            finally
            {
                this.Closing -= gameWindowClosing;
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    this.Close();
                });
            }
        }
    }
}