// MindWeaveClient/ViewModel/Main/SelectAvatarViewModel.cs

using MindWeaveClient.ProfileService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    /// <summary>
    /// Representa un único avatar en la galería de selección.
    /// </summary>
    public class Avatar : BaseViewModel
    {
        public string imagePath { get; set; }
    }

    /// <summary>
    /// ViewModel para la pantalla de selección de avatares precargados.
    /// </summary>
    public class SelectAvatarViewModel : BaseViewModel
    {
        private readonly Action navigateBack;

        // --- Backing Fields ---
        private ObservableCollection<Avatar> availableAvatarsValue;
        private Avatar selectedAvatarValue;

        // --- Public Properties (camelCase) ---
        public ObservableCollection<Avatar> availableAvatars { get => availableAvatarsValue; set { availableAvatarsValue = value; OnPropertyChanged(); } }
        public Avatar selectedAvatar { get => selectedAvatarValue; set { selectedAvatarValue = value; OnPropertyChanged(); } }

        // --- Comandos (camelCase) ---
        public ICommand saveSelectionCommand { get; }
        public ICommand cancelCommand { get; }

        public SelectAvatarViewModel(Action navigateBack)
        {
            this.navigateBack = navigateBack;
            cancelCommand = new RelayCommand(p => this.navigateBack?.Invoke());
            saveSelectionCommand = new RelayCommand(async p => await saveSelection(), p => canSave());

            loadAvailableAvatars();
        }

        private void loadAvailableAvatars()
        {
            // --- Lógica para cargar los avatares de la carpeta Resources ---
            availableAvatars = new ObservableCollection<Avatar>();

            // OJO: Esta es una forma simple de hacerlo. Si tienes muchos avatares,
            // se podría hacer de forma más dinámica, pero para empezar es perfecto.
            var avatarPaths = new string[]
            {
                "/Resources/Images/Avatar/default_avatar.png",
                "/Resources/Images/Avatar/alien_avatar.png",
                "/Resources/Images/Avatar/goblin_avatar.png",
                "/Resources/Images/Avatar/ball_avatar.png",
                "/Resources/Images/Avatar/pirat_avatar.png",
                "/Resources/Images/Avatar/robot_avatar.png",
                // Añade aquí las rutas al resto de tus avatares precargados
                // Ejemplo: "/Resources/Images/Avatar/avatar_warrior.png",
                // Ejemplo: "/Resources/Images/Avatar/avatar_mage.png",
            };

            foreach (var path in avatarPaths)
            {
                availableAvatars.Add(new Avatar { imagePath = path });
            }

            // Seleccionamos el avatar actual del jugador, si está en la lista
            selectedAvatar = availableAvatars.FirstOrDefault(a => a.imagePath == SessionService.avatarPath);
        }

        private bool canSave()
        {
            // Solo se puede guardar si se ha seleccionado un avatar
            return selectedAvatar != null;
        }

        private async Task saveSelection()
        {
            if (!canSave()) return;

            try
            {
                var client = new ProfileManagerClient();
                // Llamamos al nuevo método del servidor, pasándole solo la ruta (string)
                var result = await client.updateAvatarPathAsync(SessionService.username, selectedAvatar.imagePath);

                if (result.success)
                {
                    // Actualizamos la sesión local para que el cambio se vea al instante
                    SessionService.updateAvatarPath(selectedAvatar.imagePath);
                    MessageBox.Show(result.message, "Success", MessageBoxButton.OK, MessageBoxImage.Information); // TO-DO: Lang
                    navigateBack?.Invoke(); // Volvemos a la pantalla anterior
                }
                else
                {
                    MessageBox.Show(result.message, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}