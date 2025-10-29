// MindWeaveClient/View/Dialogs/GuestInputDialog.xaml.cs
using System.Windows;

namespace MindWeaveClient.View.Dialogs
{
    public partial class GuestInputDialog : Window
    {
        public string GuestEmail { get; private set; }

        public GuestInputDialog()
        {
            InitializeComponent();
            // Opcional: Poner el foco en el TextBox al abrir
            Loaded += (s, e) => EmailTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            GuestEmail = EmailTextBox.Text;
            this.DialogResult = true; // Indica que se aceptó
            this.Close();
        }

        // El botón Cancelar cerrará la ventana automáticamente debido a IsCancel="True"
    }
}