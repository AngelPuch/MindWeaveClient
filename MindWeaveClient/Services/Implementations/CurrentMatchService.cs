using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;

namespace MindWeaveClient.Services.Implementations
{
    public class CurrentMatchService : ICurrentMatchService
    {
        private string matchId;
        private PuzzleDefinitionDto currentPuzzle;

        public PuzzleDefinitionDto getCurrentPuzzle()
        {
            return currentPuzzle;
        }
        public void setPuzzle(PuzzleDefinitionDto puzzle)
        {
            this.currentPuzzle = puzzle;
        }
        public void setMatchId(string matchId)
        {
            this.matchId = matchId;
        }

        public string getMatchId()
        {
            var id = matchId;
            matchId = null;
            return id;
        }
    }
}
