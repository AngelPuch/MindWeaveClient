using MindWeaveClient.Services;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Services.Callbacks;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MindWeaveClient.View.Main;

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
        private readonly INavigationService navigationService;
        private ResultDisplayItem winner;
        private ObservableCollection<ResultDisplayItem> allParticipants;


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

        public PostMatchResultsViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
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

                if (Winner == null && AllParticipants.Count > 0)
                {
                    Winner = AllParticipants[0];
                }
            });
        }

        private void executeGoToMainMenu(object obj)
        {
            navigationService.navigateTo<MainMenuPage>();
        }
    }
}