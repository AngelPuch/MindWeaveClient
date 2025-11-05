// MindWeaveClient/View/Authentication/GuestJoinPage.xaml.cs
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.ViewModel.Authentication;
using MindWeaveClient.ViewModel.Game;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            viewModel.JoinSuccess += onJoinSuccess; // Suscribirse al evento de éxito

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

            var lobbyPage = new LobbyPage();
            lobbyPage.DataContext = new LobbyViewModel(
                e.initialLobbyState,
                page => mainAppWindow.MainFrame.Navigate(page),
                () => mainAppWindow.MainFrame.Navigate(new MainMenuPage(page => mainAppWindow.MainFrame.Navigate(page)))
            );

            mainAppWindow.MainFrame.Navigate(lobbyPage);

            mainAppWindow.Show();
            currentWindow?.Close();
        }

    }
}