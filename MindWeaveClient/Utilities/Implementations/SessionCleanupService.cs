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
        public SessionCleanupService(
            IAuthenticationService authenticationService,
            ISocialService socialService,
            IMatchmakingService matchmakingService,
            ICurrentMatchService currentMatchService)
        {
            this.authenticationService = authenticationService;
            this.socialService = socialService;
            this.matchmakingService = matchmakingService;
            this.currentMatchService = currentMatchService;
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
            }
            catch (CommunicationException)
            {
            }
            catch (TimeoutException)
            {
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            finally
            {
                SessionService.clearSession();
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
            }
            catch (CommunicationException)
            {
            }
            catch (TimeoutException)
            {
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }

            finally
            {
                await cleanUpSessionAsync();
            }
        }
    }
}
