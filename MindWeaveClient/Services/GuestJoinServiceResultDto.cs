using MindWeaveClient.MatchmakingService;

namespace MindWeaveClient.Services
{
    public class GuestJoinServiceResultDto
    {
        public GuestJoinResultDto wcfResult { get; }
        public bool isSocialServiceConnected { get; }
        public bool didMatchmakingConnect { get; }

        public GuestJoinServiceResultDto(GuestJoinResultDto wcfResult, bool socialConnected, bool matchmakingConnected = true)
        {
            this.wcfResult = wcfResult;
            this.isSocialServiceConnected = socialConnected;
            this.didMatchmakingConnect = matchmakingConnected;
        }
    }
}
