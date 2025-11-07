using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;

namespace MindWeaveClient.Services.Implementations
{
    public class CurrentLobbyService : ICurrentLobbyService
    {
        private LobbyStateDto initialState;

        public void setInitialState(LobbyStateDto initialState)
        {
            this.initialState = initialState;
        }

        public LobbyStateDto getInitialState()
        {
            var state = initialState;
            initialState = null;
            return state;
        }
    }
}
