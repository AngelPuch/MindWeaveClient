using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MindWeaveClient.Services.Implementations
{
    public class CurrentMatchService : ICurrentMatchService
    {
        private string matchId;
        private PuzzleManagerService.PuzzleDefinitionDto currentPuzzle;

        public string LobbyId { get; private set; }
        public List<string> Players { get; private set; }
        public LobbySettingsDto CurrentSettings { get; private set; }
        public string PuzzleImagePath { get; private set; }

        public event EventHandler OnMatchFound;
        public event Action PuzzleReady;

        public void initializeMatch(string lobbyId, List<string> players, LobbySettingsDto settings, string puzzleImagePath)
        {
            this.LobbyId = lobbyId;
            this.Players = players;
            this.CurrentSettings = settings;
            this.PuzzleImagePath = puzzleImagePath;

            this.matchId = lobbyId;
            Debug.WriteLine($"[CurrentMatchService] Match initialized: LobbyId={lobbyId}, Players={string.Join(", ", players)}");

            OnMatchFound?.Invoke(this, EventArgs.Empty);
        }

        public PuzzleManagerService.PuzzleDefinitionDto getCurrentPuzzle()
        {
            return currentPuzzle;
        }

        public void setPuzzle(PuzzleManagerService.PuzzleDefinitionDto puzzle)
        {
            this.currentPuzzle = puzzle;
            Debug.WriteLine($"[CurrentMatchService] Puzzle set with {puzzle?.Pieces?.Length ?? 0} pieces");

            PuzzleReady?.Invoke();
        }

        public void setMatchId(string matchId)
        {
            this.matchId = matchId;
            if (string.IsNullOrEmpty(this.LobbyId))
            {
                this.LobbyId = matchId;
            }
            Debug.WriteLine($"[CurrentMatchService] MatchId set to: {matchId}");

        }

        public string getMatchId()
        {
            return matchId;

        }
    }
}