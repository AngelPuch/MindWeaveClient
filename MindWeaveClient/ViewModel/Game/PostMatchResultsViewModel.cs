using MindWeaveClient.Properties.Langs; // Para recursos
using MindWeaveClient.Services;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Services.Callbacks;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;

namespace MindWeaveClient.ViewModel.Game
{
    // Clase Wrapper para adaptar el DTO a tu XAML
    public class ResultDisplayItem
    {
        public int FinalRank { get; set; } // Mapea a Rank
        public string Username { get; set; }
        public int Score { get; set; }
        public int PiecesPlaced { get; set; }
        public string AvatarPath { get; set; }
        public bool IsWinner { get; set; }
    }

    public class PostMatchResultsViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private ResultDisplayItem _winner;
        private ObservableCollection<ResultDisplayItem> _allParticipants;

        public PostMatchResultsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _allParticipants = new ObservableCollection<ResultDisplayItem>();

            // Inicializar comandos
            BackToLobbyCommand = new RelayCommand(ExecuteBackToLobby);
            GoToMainMenuCommand = new RelayCommand(ExecuteGoToMainMenu);

            // Cargar datos automáticamente desde el CallbackHandler
            LoadData();
        }

        // Propiedades para tu XAML
        public ResultDisplayItem Winner
        {
            get => _winner;
            set { _winner = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ResultDisplayItem> AllParticipants
        {
            get => _allParticipants;
            set { _allParticipants = value; OnPropertyChanged(); }
        }

        public ICommand BackToLobbyCommand { get; }
        public ICommand GoToMainMenuCommand { get; }

        private void LoadData()
        {
            var results = MatchmakingCallbackHandler.LastMatchResults;

            if (results == null || results.PlayerResults == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                AllParticipants.Clear();

                foreach (var playerDto in results.PlayerResults.OrderBy(p => p.Rank))
                {
                    // Lógica de Avatar: Usar el local si soy yo, o uno default/caché para otros
                    string avatar = "/Resources/Images/Avatar/default_avatar.png";
                    if (playerDto.Username == SessionService.Username) // O comparar ID
                    {
                        // Asumiendo que SessionService tiene el path guardado
                        avatar = SessionService.AvatarPath ?? avatar;
                    }
                    // TODO: Si tienes los avatares de los amigos cacheados en CurrentMatchService, úsalos aquí.

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

                    // Asignar al ganador para el bloque superior del XAML
                    if (displayItem.IsWinner)
                    {
                        Winner = displayItem;
                    }
                }

                // Fallback por si no viene ganador explícito (empate raro)
                if (Winner == null && AllParticipants.Count > 0)
                {
                    Winner = AllParticipants[0];
                }
            });
        }

        private void ExecuteBackToLobby(object obj)
        {
            // Aquí decides si vuelves al Lobby (si el servidor lo permite) o al menú
            _navigationService.navigateTo<LobbyPage>();
        }

        private void ExecuteGoToMainMenu(object obj)
        {
            _navigationService.navigateTo<MainMenuPage>();
        }
    }
}