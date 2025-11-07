using MindWeaveClient.MatchmakingService;

namespace MindWeaveClient.Services.Abstractions
{
    public interface ICurrentLobbyService
    {
        /// <summary>
        /// Sets the initial state of the lobby to be joined/created.
        /// This is set by the calling ViewModel (e.g., MainMenuViewModel).
        /// </summary>
        /// <param name="initialState">The initial lobby state (if creating) or null (if joining).</param>
        void setInitialState(LobbyStateDto initialState);

        /// <summary>
        /// Retrieves the initial lobby state.
        /// This is called by the LobbyViewModel upon its creation.
        /// </summary>
        /// <returns>The initial lobby state or null.</returns>
        LobbyStateDto getInitialState();
    }
}
