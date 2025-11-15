
using MindWeaveClient.MatchmakingService;

namespace MindWeaveClient.Services.Abstractions
{
    public interface ICurrentMatchService
    {
        void setMatchId(string matchId);

        string getMatchId();

        PuzzleDefinitionDto getCurrentPuzzle();
        void setPuzzle(PuzzleDefinitionDto puzzle);
    }
}
