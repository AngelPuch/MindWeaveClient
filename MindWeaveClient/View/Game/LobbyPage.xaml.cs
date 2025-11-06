// MindWeaveClient/View/Game/LobbyPage.xaml.cs
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.ViewModel.Game;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.View.Game
{
    public partial class LobbyPage : Page
    {
        public LobbyPage(LobbyStateDto initialState, Action<Page> navigateToAction, Action navigateBackAction)
        {
            InitializeComponent();

            var viewModel = new LobbyViewModel(initialState, navigateToAction, navigateBackAction);

            viewModel.OnMatchStarting += onMatchStarting;
                
            DataContext = viewModel;
        }

        private void onMatchStarting(object sender, string matchId)
        {
            var gameWindow = new GameWindow();
            // gameWindow.DataContext = new GameViewModel(matchId);
            gameWindow.Show();

            var currentWindow = Window.GetWindow(this);
            currentWindow?.Close();
        }
    }
}