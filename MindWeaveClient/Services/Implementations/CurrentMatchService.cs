using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MindWeaveClient.Services.Implementations
{
    public class CurrentMatchService : ICurrentMatchService
    {
        private static string matchId;
        public Guid MatchId { get; set; }
        private static string lobbyId;
        private static List<string> players;
        private static LobbySettingsDto currentSettings;
        private static string puzzleImagePath;
        private static PuzzleManagerService.PuzzleDefinitionDto currentPuzzle;

        public event EventHandler OnMatchFound;
        public event Action PuzzleReady;

        public bool IsMatchActive => MatchId != Guid.Empty;
        public string LobbyId
        {
            get => lobbyId;
            private set => lobbyId = value;
        }

        public List<string> Players
        {
            get => players;
            private set => players = value;
        }

        public LobbySettingsDto CurrentSettings
        {
            get => currentSettings;
            private set => currentSettings = value;
        }

        public string PuzzleImagePath
        {
            get => puzzleImagePath;
            private set => puzzleImagePath = value;
        }

        public void initializeMatch(string lobbyId, List<string> players, LobbySettingsDto settings, string puzzleImagePath)
        {
            CurrentMatchService.lobbyId = lobbyId;
            CurrentMatchService.players = players;
            currentSettings = settings;
            CurrentMatchService.puzzleImagePath = puzzleImagePath;

            CurrentMatchService.matchId = lobbyId;
            OnMatchFound?.Invoke(this, EventArgs.Empty);
        }

        public PuzzleManagerService.PuzzleDefinitionDto getCurrentPuzzle()
        {
            return currentPuzzle;
        }

        public void setPuzzle(PuzzleManagerService.PuzzleDefinitionDto puzzle)
        {
            CurrentMatchService.currentPuzzle = puzzle;
            PuzzleReady?.Invoke();
        }

        public void setMatchId(string matchId)
        {
            CurrentMatchService.matchId = matchId;

            if (string.IsNullOrEmpty(lobbyId))
            {
                lobbyId = matchId;
            }
        }

        public string getMatchId()
        {
            return CurrentMatchService.matchId;
        }

        public void clearMatchData()
        {
            matchId = null;
            lobbyId = null;
            players = null;
            currentSettings = null;
            puzzleImagePath = null;
            currentPuzzle = null;
        }
    }
}