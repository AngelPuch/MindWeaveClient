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
using System.Windows.Shapes;
using MindWeaveClient.Utilities.Abstractions;

namespace MindWeaveClient.View.Game
{
    public partial class GameWindow : Window
    {
        private readonly INavigationService navigationService;

        public GameWindow(INavigationService navigationService, LobbyPage startPage)
        {
            InitializeComponent();
            this.navigationService = navigationService;

            this.navigationService.initialize(GameFrame);
            GameFrame.Content = startPage;
        }
    }
}
