// MindWeaveClient/View/Game/LobbyPage.xaml.cs
using MindWeaveClient.ViewModel.Game;
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.View.Game
{
    public partial class LobbyPage : Page
    {
        public LobbyPage()
        {
            InitializeComponent();
            this.Unloaded += LobbyPage_Unloaded;
        }

        private void LobbyPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LobbyViewModel viewModel)
            {
                viewModel.cleanup();
            }
            this.Unloaded -= LobbyPage_Unloaded;
        }

        // YA NO NECESITAS LAS CLASES DE CONVERTIDORES AQUÍ
    }
}