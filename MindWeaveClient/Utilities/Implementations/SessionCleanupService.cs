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
            catch (EndpointNotFoundException)
            {
                // ignore
            }
            catch (CommunicationException)
            {
                // ignore
            }
            catch (TimeoutException)
            {
                // ignore
            }
            catch (SocketException)
            {
                // ignore
            }
            catch (ObjectDisposedException)
            {
                // ignore
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
            catch (EndpointNotFoundException)
            {
                // Ignore
            }
            catch (CommunicationException)
            {
                // Ignore
            }
            catch (TimeoutException)
            {
                // Ignore
            }
            catch (SocketException)
            {
                // Ignore
            }
            catch (ObjectDisposedException)
            {
                // Ignore
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
            catch (EndpointNotFoundException)
            {
                // Ignore
            }
            catch (CommunicationException)
            {
                // Ignore
            }
            catch (TimeoutException)
            {
                // Ignore
            }
            catch (SocketException)
            {
                // Ignore
            }
            catch (ObjectDisposedException)
            {
                // Ignore
            }
            finally
            {
                await cleanUpSessionAsync();
            }
        }

    }
}