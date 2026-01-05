using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Main;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace MindWeaveClient.View.Authentication
{
    public partial class AuthenticationWindow : Window
    {
        private readonly INavigationService navigationService;
        private readonly IDialogService dialogService;

        private bool isExitDialogShown = false;

        public bool IsExitConfirmed { get; set; } = false;

        public AuthenticationWindow(
            INavigationService navigationService,
            LoginPage startPage,
            IDialogService dialogService) 
        {
            InitializeComponent();

            this.navigationService = navigationService;
            this.dialogService = dialogService;
            this.navigationService.initialize(AuthenticationFrame);

            AuthenticationFrame.Navigate(startPage);
        }

        private void windowClosing(object sender, CancelEventArgs e)
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

           
            bool isTransitioningToMain = Application.Current.Windows
                .OfType<MainWindow>()
                .Any();

            if (isTransitioningToMain)
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
                IsExitConfirmed = true;
                Application.Current.Shutdown();
            }
        }
    }
}