using MindWeaveClient.ViewModel.Main;
using System;
using System.Windows.Controls;

namespace MindWeaveClient.View.Main
{
    public partial class MainMenuPage : Page
    {
        public MainMenuPage(Action<Page> navigateAction)
        {
            InitializeComponent();
            DataContext = new MainMenuViewModel(navigateAction, this);
        }
    }
}
