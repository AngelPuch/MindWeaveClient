using MindWeaveClient.View.Game;
using System.Windows;

namespace MindWeaveClient.View.Main
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new MainMenuPage(page => MainFrame.Navigate(page)));
        }
    }
}
