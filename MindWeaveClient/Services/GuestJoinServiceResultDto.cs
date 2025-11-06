using MindWeaveClient.MatchmakingService;

namespace MindWeaveClient.Services
{
    public class GuestJoinServiceResultDto
    {
        public GuestJoinResultDto wcfResult { get; }
        public bool didMatchmakingConnect { get; }

        public GuestJoinServiceResultDto(GuestJoinResultDto wcfResult, bool matchmakingConnected = true)
        {
            this.wcfResult = wcfResult;
            this.didMatchmakingConnect = matchmakingConnected;
        }
    }
}
