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
        public GuestJoinPage(GuestJoinViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void CodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^a-zA-Z0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}