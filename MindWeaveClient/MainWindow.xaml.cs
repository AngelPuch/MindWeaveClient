using MindWeaveClient.View.Game; 
using System.Windows;

namespace MindWeaveClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LobbyPage());
        }
    }
}