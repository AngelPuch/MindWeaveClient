// MindWeaveClient/ViewModel/Main/MainMenuViewModel.cs

using MindWeaveClient.Services;
using MindWeaveClient.View.Main;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MindWeaveClient.ViewModel.Main
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly Action<Page> navigateTo;
        private readonly Page mainMenuPage;

        public string playerUsername { get; }
        public string playerAvatarPath { get; }

        public ICommand profileCommand { get; }
        public ICommand playCommand { get; }
        public ICommand socialCommand { get; }
        public ICommand settingsCommand { get; }

        public MainMenuViewModel(Action<Page> navigateTo, Page mainMenuPage)
        {
            this.navigateTo = navigateTo;
            this.mainMenuPage = mainMenuPage;

            playerUsername = SessionService.username;
            playerAvatarPath = SessionService.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

            profileCommand = new RelayCommand(p => executeGoToProfile());
            socialCommand = new RelayCommand(p => executeGoToSocial());
            // TO-DO: Implementar otros comandos
        }

        private void executeGoToProfile()
        {
            var profilePage = new ProfilePage();
            // Le pasamos al ProfileViewModel las dos instrucciones que necesita
            profilePage.DataContext = new ProfileViewModel(
                () => navigateTo(mainMenuPage),      // 1. Para volver al menú
                () => executeGoToEditProfile()       // 2. Para ir a editar
            );
            navigateTo(profilePage);
        }

        private void executeGoToSocial() {
            navigateTo(new SocialPage());
        }

        private void executeGoToEditProfile()
        {
            var editProfilePage = new EditProfilePage();
            // Ahora le pasamos al EditProfileViewModel las DOS instrucciones que pide
            editProfilePage.DataContext = new EditProfileViewModel(
                () => executeGoToProfile(),          // 1. Para cancelar y volver al perfil
                () => executeGoToSelectAvatar()      // 2. Para cambiar el avatar
            );
            navigateTo(editProfilePage);
        }

        private void executeGoToSelectAvatar()
        {
            var selectAvatarPage = new SelectAvatarPage(); // <-- Aún no hemos creado este archivo XAML
                                                           // Creamos el ViewModel de la galería y le pasamos la instrucción para volver a la página de edición
            selectAvatarPage.DataContext = new SelectAvatarViewModel(
                () => executeGoToEditProfile()
            );
            navigateTo(selectAvatarPage);
        }

    }
}