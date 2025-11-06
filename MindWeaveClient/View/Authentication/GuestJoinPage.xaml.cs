using MindWeaveClient.MatchmakingService;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.ViewModel.Authentication;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MindWeaveClient.Services;

namespace MindWeaveClient.View.Authentication
{
    public partial class GuestJoinPage : Page
    {
        public GuestJoinPage(Action<Page> navigateAction)
        {
            InitializeComponent();

            Action navigateBackAction = () =>
            {
                navigateAction(new LoginPage(navigateAction));
            };

            var viewModel = new GuestJoinViewModel(navigateBackAction);
            viewModel.JoinSuccess += onJoinSuccess;

            DataContext = viewModel;
        }

        private void CodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^a-zA-Z0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void onJoinSuccess(object sender, GuestJoinResultDto e)
        {
            var currentWindow = Window.GetWindow(this);
            var mainAppWindow = new MainWindow();
            Action<Page> navigateForwardAction = page => mainAppWindow.MainFrame.Navigate(page);

            Action guestNavigateBackAction = () =>
            {
                SessionService.ClearSession();
                var authWindow = new AuthenticationWindow();
                authWindow.Show();
                mainAppWindow.Close();
            };

            var lobbyPage = new LobbyPage(
                e.initialLobbyState, 
                navigateForwardAction,
                guestNavigateBackAction);

            mainAppWindow.MainFrame.Navigate(lobbyPage);
            mainAppWindow.Show();
            currentWindow?.Close();
        }

    }
}