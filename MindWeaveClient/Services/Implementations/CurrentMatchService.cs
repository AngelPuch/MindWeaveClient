using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MindWeaveClient.Services.Implementations
{
    public class CurrentMatchService : ICurrentMatchService
    {
        // Campos estáticos para persistencia (solución al bug de LobbyId nulo)
        // Nomenclatura: camelCase sin guiones bajos según el estándar
        private static string matchId;
        private static string lobbyId;
        private static List<string> players;
        private static LobbySettingsDto currentSettings;
        private static string puzzleImagePath;
        private static PuzzleManagerService.PuzzleDefinitionDto currentPuzzle;

        public event EventHandler OnMatchFound;
        public event Action PuzzleReady;

        // Propiedades públicas (PascalCase) vinculadas a los campos estáticos
        public string LobbyId
        {
            get { return CurrentMatchService.lobbyId; }
            private set { CurrentMatchService.lobbyId = value; }
        }

        public List<string> Players
        {
            get { return CurrentMatchService.players; }
            private set { CurrentMatchService.players = value; }
        }

        public LobbySettingsDto CurrentSettings
        {
            get { return CurrentMatchService.currentSettings; }
            private set { CurrentMatchService.currentSettings = value; }
        }

        public string PuzzleImagePath
        {
            get { return CurrentMatchService.puzzleImagePath; }
            private set { CurrentMatchService.puzzleImagePath = value; }
        }

        public void initializeMatch(string lobbyId, List<string> players, LobbySettingsDto settings, string puzzleImagePath)
        {
            CurrentMatchService.lobbyId = lobbyId;
            CurrentMatchService.players = players;
            CurrentMatchService.currentSettings = settings;
            CurrentMatchService.puzzleImagePath = puzzleImagePath;

            CurrentMatchService.matchId = lobbyId;

            Debug.WriteLine($"[CurrentMatchService] Match initialized: LobbyId={lobbyId}, Players={string.Join(", ", players)}");

            OnMatchFound?.Invoke(this, EventArgs.Empty);
        }

        public PuzzleManagerService.PuzzleDefinitionDto getCurrentPuzzle()
        {
            return CurrentMatchService.currentPuzzle;
        }

        public void setPuzzle(PuzzleManagerService.PuzzleDefinitionDto puzzle)
        {
            CurrentMatchService.currentPuzzle = puzzle;
            Debug.WriteLine($"[CurrentMatchService] Puzzle set with {puzzle?.Pieces?.Length ?? 0} pieces");

            PuzzleReady?.Invoke();
        }

        public void setMatchId(string matchId)
        {
            CurrentMatchService.matchId = matchId;

            if (string.IsNullOrEmpty(CurrentMatchService.lobbyId))
            {
                CurrentMatchService.lobbyId = matchId;
            }

            Debug.WriteLine($"[CurrentMatchService] MatchId set to: {matchId}");
        }

        public string getMatchId()
        {
            return CurrentMatchService.matchId;
        }
    }
}