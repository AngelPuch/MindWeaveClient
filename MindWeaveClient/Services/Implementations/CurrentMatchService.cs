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
        private static string lobbyId;
        private static List<string> players;
        private static LobbySettingsDto currentSettings;
        private static string puzzleImagePath;
        private static PuzzleManagerService.PuzzleDefinitionDto currentPuzzle;

        public event EventHandler OnMatchFound;
        public event Action PuzzleReady;

        public string LobbyId
        {
            get => CurrentMatchService.lobbyId;
            private set => CurrentMatchService.lobbyId = value;
        }

        public List<string> Players
        {
            get => CurrentMatchService.players;
            private set => CurrentMatchService.players = value;
        }

        public LobbySettingsDto CurrentSettings
        {
            get => CurrentMatchService.currentSettings;
            private set => CurrentMatchService.currentSettings = value;
        }

        public string PuzzleImagePath
        {
            get => CurrentMatchService.puzzleImagePath;
            private set => CurrentMatchService.puzzleImagePath = value;
        }

        public void initializeMatch(string lobbyId, List<string> players, LobbySettingsDto settings, string puzzleImagePath)
        {
            CurrentMatchService.lobbyId = lobbyId;
            CurrentMatchService.players = players;
            CurrentMatchService.currentSettings = settings;
            CurrentMatchService.puzzleImagePath = puzzleImagePath;

            CurrentMatchService.matchId = lobbyId;
            OnMatchFound?.Invoke(this, EventArgs.Empty);
        }

        public PuzzleManagerService.PuzzleDefinitionDto getCurrentPuzzle()
        {
            return CurrentMatchService.currentPuzzle;
        }

        public void setPuzzle(PuzzleManagerService.PuzzleDefinitionDto puzzle)
        {
            CurrentMatchService.currentPuzzle = puzzle;
            PuzzleReady?.Invoke();
        }

        public void setMatchId(string matchId)
        {
            CurrentMatchService.matchId = matchId;

            if (string.IsNullOrEmpty(CurrentMatchService.lobbyId))
            {
                CurrentMatchService.lobbyId = matchId;
            }
        }

        public string getMatchId()
        {
            return CurrentMatchService.matchId;
        }

        public void clearMatchData()
        {
            CurrentMatchService.matchId = null;
            CurrentMatchService.lobbyId = null;
            CurrentMatchService.players = null;
            CurrentMatchService.currentSettings = null;
            CurrentMatchService.puzzleImagePath = null;
            CurrentMatchService.currentPuzzle = null;
        }
    }
}