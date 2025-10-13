using MindWeaveClient.Services;
using MindWeaveClient.View.Main;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly Action<Page> _navigateTo;

        public string playerUsername { get; }
        public string playerAvatarPath { get; }

        public ICommand profileCommand { get; }
        public ICommand playCommand { get; }
        public ICommand socialCommand { get; }
        public ICommand settingsCommand { get; }

        public MainMenuViewModel(Action<Page> navigateTo)
        {
            _navigateTo = navigateTo;
            playerUsername = SessionService.username;
            playerAvatarPath = SessionService.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

            profileCommand = new RelayCommand(p => executeGoToProfile());
            // TO-DO: Implement other commands
        }

        private void executeGoToProfile()
        {
            _navigateTo(new ProfilePage());
        }
    }
}