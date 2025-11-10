
namespace MindWeaveClient.Services.Abstractions
{
    public interface ICurrentMatchService
    {
        void setMatchId(string matchId);

        string getMatchId();
    }
}
