using MindWeaveClient.Services.Abstractions;

namespace MindWeaveClient.Services.Implementations
{
    public class CurrentMatchService : ICurrentMatchService
    {
        private string matchId;

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
