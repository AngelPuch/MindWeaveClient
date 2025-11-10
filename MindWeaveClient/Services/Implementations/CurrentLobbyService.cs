using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;

namespace MindWeaveClient.Services.Implementations
{
    public class CurrentLobbyService : ICurrentLobbyService
    {
        private LobbyStateDto initialState;
        private bool hasBeenRetrieved = false;

        public void setInitialState(LobbyStateDto initialState)
        {
            this.initialState = initialState;
            this.hasBeenRetrieved = false;
        }

        public LobbyStateDto getInitialState()
        {
            if (hasBeenRetrieved)
            {
                return null;
            }

            hasBeenRetrieved = true;
            return initialState;
        }

        public void clearState()
        {
            initialState = null;
            hasBeenRetrieved = false;
        }
    }
}