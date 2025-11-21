using MindWeaveClient.ViewModel.Game;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.View.Game
{
    public partial class LobbyPage : Page
    {
        public LobbyPage(LobbyViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.Unloaded += lobbyPageUnloaded;
        }

        private void lobbyPageUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}