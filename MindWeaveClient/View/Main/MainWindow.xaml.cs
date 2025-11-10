using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System.Windows;

namespace MindWeaveClient.View.Main
{
    public partial class MainWindow : Window
    {
        private readonly INavigationService navigationService;
        private readonly IInvitationService invitationService;

        public MainWindow(INavigationService navigationService, IInvitationService invitationService)
        {
            InitializeComponent();
            this.navigationService = navigationService;
            this.invitationService = invitationService;

            this.navigationService.initialize(MainFrame);
            this.navigationService.navigateTo<MainMenuPage>();

            this.Loaded += mainWindow_Loaded;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            invitationService.subscribeToGlobalInvites();
        }
    }
}