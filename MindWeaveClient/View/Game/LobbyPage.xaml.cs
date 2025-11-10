using MindWeaveClient.ViewModel.Game;
using System.Windows.Controls;

namespace MindWeaveClient.View.Game
{
    public partial class LobbyPage : Page
    {
        public LobbyPage(LobbyViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}