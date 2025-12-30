using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Main;
using System;
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
        private const string DEFAULT_AVATAR_PATH = "/Resources/Images/Avatar/default_avatar.png";
        private const string DRAW_TITLE = "¡EMPATE!";


        private readonly IWindowNavigationService windowNavigationService;
        private readonly ICurrentMatchService currentMatchService;

        private ResultDisplayItem winner;
        private ObservableCollection<ResultDisplayItem> allParticipants;
        private string resultTitle;
        private Visibility winnerSectionVisibility;
        private Visibility drawSectionVisibility;


        public string ResultTitle
        {
            get => resultTitle;
            set { resultTitle = value; OnPropertyChanged(); }
        }

        public Visibility WinnerSectionVisibility
        {
            get => winnerSectionVisibility;
            set { winnerSectionVisibility = value; OnPropertyChanged(); }
        }

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
            this.windowNavigationService = windowNavigationService ?? throw new ArgumentNullException(nameof(windowNavigationService));
            this.currentMatchService = currentMatchService ?? throw new ArgumentNullException(nameof(currentMatchService));

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
                    string avatarToDisplay = !string.IsNullOrEmpty(playerDto.AvatarPath)
                        ? playerDto.AvatarPath
                        : DEFAULT_AVATAR_PATH;

                    var displayItem = new ResultDisplayItem
                    {
                        FinalRank = playerDto.Rank,
                        Username = playerDto.Username,
                        Score = playerDto.Score,
                        PiecesPlaced = playerDto.PiecesPlaced,
                        IsWinner = playerDto.IsWinner,
                        AvatarPath = avatarToDisplay
                    };

                    AllParticipants.Add(displayItem);

                    if (displayItem.IsWinner)
                    {
                        Winner = displayItem;
                    }
                }

                updateResultDisplay();
            });
        }

        private void updateResultDisplay()
        {
            if (Winner != null)
            {
                ResultTitle = Lang.PostMatchLbWinner;
                WinnerSectionVisibility = Visibility.Visible;
                DrawSectionVisibility = Visibility.Collapsed;
            }
            else
            {
                ResultTitle = DRAW_TITLE;
                WinnerSectionVisibility = Visibility.Collapsed;
                DrawSectionVisibility = Visibility.Visible;
            }
        }

        private void executeGoToMainMenu(object obj)
        {
            markGameAsEndedNaturally();
            clearMatchData();
            navigateToMainMenu();
        }

        private void markGameAsEndedNaturally()
        {
            var gameWindow = Application.Current.Windows.OfType<GameWindow>().FirstOrDefault();
            if (gameWindow != null)
            {
                gameWindow.GameEndedNaturally = true;
            }
        }

        private void clearMatchData()
        {
            currentMatchService.clearMatchData();
        }

        private void navigateToMainMenu()
        {
            var gameWindow = Application.Current.Windows.OfType<GameWindow>().FirstOrDefault();
            if (gameWindow != null)
            {
                gameWindow.IsExitConfirmed = true;
            }

            windowNavigationService.openWindow<MainWindow>();
            windowNavigationService.closeWindow<GameWindow>();
        }
    }
}