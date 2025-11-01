using MindWeaveClient.ViewModel.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MindWeaveClient.View.Main
{
    /// <summary>
    /// Lógica de interacción para EditProfilePage.xaml
    /// </summary>
    public partial class EditProfilePage : Page
    {
        public EditProfilePage()
        {
            InitializeComponent();
            this.Loaded += EditProfilePage_Loaded;
        }


        private void EditProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            // Cuando la página se carga (o se vuelve a mostrar),
            // le decimos al ViewModel que refresque sus datos.
            if (this.DataContext is EditProfileViewModel vm)
            {
                // Podrías crear un método RefreshData() en el ViewModel
                // o simplemente volver a llamar loadEditableData si es seguro hacerlo.
                // Por ahora, solo actualizaremos el avatar que es lo que cambió.
                vm.RefreshAvatar(); // Necesitas añadir este método al ViewModel
            }
        }
    }
}
