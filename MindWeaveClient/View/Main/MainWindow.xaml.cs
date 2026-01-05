using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MindWeaveClient.View.Game;

namespace MindWeaveClient.View.Main
{
    public partial class MainWindow : Window
    {
        private readonly ISessionCleanupService cleanupService;
        private readonly IInvitationService invitationService;
        private readonly IDialogService dialogService;

        private bool isExitDialogShown = false;
        public bool IsExitConfirmed { get; set; } = false;

        public MainWindow(
            INavigationService navigationService, 
            IInvitationService invitationService, 
            ISessionCleanupService cleanupService,
            IDialogService dialogService)
        {
            InitializeComponent();
            this.invitationService = invitationService;
            this.cleanupService = cleanupService;
            this.dialogService = dialogService;

            navigationService.initialize(MainFrame);
            navigationService.navigateTo<MainMenuPage>();

            this.Loaded += mainWindow_Loaded;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            invitationService.subscribeToGlobalInvites();
        }

        private async void windowClosing(object sender, CancelEventArgs e)
        {
            if (IsExitConfirmed)
            {
                return;
            }
            if (isExitDialogShown)
            {
                e.Cancel = true;
                return;
            }

            bool isTransitioningToGame = Application.Current.Windows
                .OfType<GameWindow>()
                .Any();

            if (isTransitioningToGame)
            {
                return;
            }

            e.Cancel = true;
            isExitDialogShown = true;

            bool exitConfirmed = dialogService.showConfirmation(
                Lang.GlobalExitConfirmMessage,
                Lang.GlobalExitConfirmTitle);

            isExitDialogShown = false;

            if (exitConfirmed)
            {
                await cleanupService.cleanUpSessionAsync();
                IsExitConfirmed = true;
                Application.Current.Shutdown();
            }
        }
    }
}