using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Utilities.Implementations
{
    public class SessionCleanupService : ISessionCleanupService
    {
        private readonly IAuthenticationService authenticationService;
        private readonly ISocialService socialService;
        private readonly IMatchmakingService matchmakingService;
        private readonly ICurrentMatchService currentMatchService;
        private readonly IChatService chatService;

        public SessionCleanupService(
            IAuthenticationService authenticationService,
            ISocialService socialService,
            IMatchmakingService matchmakingService,
            ICurrentMatchService currentMatchService,
            IChatService chatService)
        {
            this.authenticationService = authenticationService;
            this.socialService = socialService;
            this.matchmakingService = matchmakingService;
            this.currentMatchService = currentMatchService;
            this.chatService = chatService;
        }

        public async Task cleanUpSessionAsync()
        {
            try
            {
                if (!SessionService.IsGuest && !string.IsNullOrEmpty(SessionService.Username))
                {
                    await authenticationService.logoutAsync(SessionService.Username);
                }

                await socialService.disconnectAsync(SessionService.Username);
                matchmakingService.disconnect();
            }
            catch (Exception)
            {
                /*
                 * Ignore: Logout and disconnect are "best effort" operations.
                 * If the server is unreachable, timed out, or the connection is already broken,
                 * we suppress the exception. The priority is to execute the 'finally' block
                 * to ensure the local client state is cleared effectively, preventing the
                 * application from remaining in an inconsistent state.
                 */
            }
            finally
            {
                currentMatchService.clearMatchData();
                SessionService.clearSession();
                await chatService.disconnectAsync(string.Empty, string.Empty, true);
                await socialService.disconnectAsync(string.Empty, true);
                matchmakingService.disconnect(true);
            }
        }

        public async Task exitGameInProcessAsync()
        {
            try
            {
                string lobbyCode = currentMatchService.LobbyId;
                if (!string.IsNullOrEmpty(SessionService.Username) && !string.IsNullOrEmpty(lobbyCode))
                {
                    await matchmakingService.leaveGameAsync(SessionService.Username, lobbyCode);
                }
            }
            catch (Exception)
            {
                /*
                 * Ignore: Attempting to notify the server that the player is leaving the game.
                 * If the network fails here, the player is effectively leaving anyway.
                 * We catch generic exceptions to ensure the local cleanup sequence proceeds
                 * without interruption.
                 */
            }
            finally
            {
                await cleanUpSessionAsync();
            }
        }

        public async Task exitLobbyAsync(string lobbyCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(SessionService.Username) && !string.IsNullOrEmpty(lobbyCode))
                {
                    await matchmakingService.leaveLobbyAsync(SessionService.Username, lobbyCode);
                }
            }
            catch (Exception)
            {
                /*
                 * Ignore: Attempting to notify the server that the player is leaving the lobby.
                 * Failure here (e.g., due to connection loss) implies the user is already disconnected
                 * from the server's perspective or soon will be.
                 * We suppress the error to prioritize local session cleanup.
                 */
            }
            finally
            {
                await cleanUpSessionAsync();
            }
        }

    }
}