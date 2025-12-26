using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Main;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MindWeaveClient.View.Game;

namespace MindWeaveClient.ViewModel.Game
{
    public class ResultDisplayItem
    {
        public int FinalRank { get; set; }
        public string Username { get; set; }
        public int Score { get; set; }
        public int PiecesPlaced { get; set; }
        public string AvatarPath { get; set; }
        public bool IsWinner { get; set; }
    }

    public class PostMatchResultsViewModel : BaseViewModel
    {
        private readonly IWindowNavigationService windowNavigationService;
        private readonly ICurrentMatchService currentMatchService;

        private ResultDisplayItem winner;
        private ObservableCollection<ResultDisplayItem> allParticipants;

        private string resultTitle;
        public string ResultTitle
        {
            get => resultTitle;
            set { resultTitle = value; OnPropertyChanged(); }
        }

        private Visibility winnerSectionVisibility;
        public Visibility WinnerSectionVisibility
        {
            get => winnerSectionVisibility;
            set { winnerSectionVisibility = value; OnPropertyChanged(); }
        }

        private Visibility drawSectionVisibility;
        public Visibility DrawSectionVisibility
        {
            get => drawSectionVisibility;
            set { drawSectionVisibility = value; OnPropertyChanged(); }
        }

        public ResultDisplayItem Winner
        {
            get => winner;
            set { winner = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ResultDisplayItem> AllParticipants
        {
            get => allParticipants;
            set { allParticipants = value; OnPropertyChanged(); }
        }
        public ICommand GoToMainMenuCommand { get; }

        public PostMatchResultsViewModel(
            IWindowNavigationService windowNavigationService,
            ICurrentMatchService currentMatchService)
        {
            this.windowNavigationService = windowNavigationService;
            this.currentMatchService = currentMatchService;

            allParticipants = new ObservableCollection<ResultDisplayItem>();

            GoToMainMenuCommand = new RelayCommand(executeGoToMainMenu);

            loadData();
        }


        private void loadData()
        {
            var results = MatchmakingCallbackHandler.LastMatchResults;

            if (results == null || results.PlayerResults == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                AllParticipants.Clear();
                Winner = null;

                foreach (var playerDto in results.PlayerResults.OrderBy(p => p.Rank))
                {
                    string avatar = "/Resources/Images/Avatar/default_avatar.png";
                    if (playerDto.Username == SessionService.Username)
                    {
                        avatar = SessionService.AvatarPath ?? avatar;
                    }

                    var displayItem = new ResultDisplayItem
                    {
                        FinalRank = playerDto.Rank,
                        Username = playerDto.Username,
                        Score = playerDto.Score,
                        PiecesPlaced = playerDto.PiecesPlaced,
                        IsWinner = playerDto.IsWinner,
                        AvatarPath = avatar
                    };

                    AllParticipants.Add(displayItem);

                    if (displayItem.IsWinner)
                    {
                        Winner = displayItem;
                    }
                }

                if (Winner != null)
                {
                    ResultTitle = Lang.PostMatchLbWinner;
                    WinnerSectionVisibility = Visibility.Visible;
                    DrawSectionVisibility = Visibility.Collapsed;
                }
                else
                {
                    ResultTitle = "¡EMPATE!";
                    WinnerSectionVisibility = Visibility.Collapsed;
                    DrawSectionVisibility = Visibility.Visible;
                }
            });
        }

        private void executeGoToMainMenu(object obj)
        {
            var gameWindow = Application.Current.Windows.OfType<GameWindow>().FirstOrDefault();
            if (gameWindow != null) { gameWindow.GameEndedNaturally = true; }

            currentMatchService.clearMatchData();
            windowNavigationService.closeWindow<GameWindow>();
            windowNavigationService.openWindow<MainWindow>();
        }
    }
}