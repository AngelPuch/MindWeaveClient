using System.Windows;

namespace MindWeaveClient.View.Dialogs
{
    public partial class GuestInputDialog : Window
    {
        public string GuestEmail { get; private set; }

        public GuestInputDialog()
        {
            InitializeComponent();
            Loaded += (s, e) => EmailTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            GuestEmail = EmailTextBox.Text;
            this.DialogResult = true;
            this.Close();
        }
    }
}