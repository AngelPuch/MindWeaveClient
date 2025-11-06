using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Utilities.Implementations;
using System;
using System.Windows;
using System.Windows.Controls;
using MindWeaveClient.Services.Callbacks;

namespace MindWeaveClient.View.Main
{
    public partial class MainWindow : Window
    {
        private readonly IDialogService dialogService;
        public MainWindow()
        {
            InitializeComponent();
            this.dialogService = new DialogService();

            GameEventAggregator.OnLobbyJoinFailed += onLobbyJoinFailed;
            MainFrame.Navigate(new MainMenuPage(page => MainFrame.Navigate(page)));
        }

        private void onLobbyJoinFailed(string reason)
        {
            dialogService.showError(reason, Lang.ErrorTitle);

            if (MainFrame.Content is Page && !(MainFrame.Content is MainMenuPage))
            {
                MainFrame.Navigate(new MainMenuPage(page => MainFrame.Navigate(page)));
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            GameEventAggregator.OnLobbyJoinFailed -= onLobbyJoinFailed;
            base.OnClosed(e);
        }
    }
}