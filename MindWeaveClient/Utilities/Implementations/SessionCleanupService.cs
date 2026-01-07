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
        // ELIMINADO: IHeartbeatService

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
            // ELIMINADO: heartbeatService
        }

        public async Task cleanUpSessionAsync()
        {
            try
            {
                // ELIMINADO: stopHeartbeatSafeAsync

                if (!SessionService.IsGuest && !string.IsNullOrEmpty(SessionService.Username))
                {
                    await authenticationService.logoutAsync(SessionService.Username);
                }

                await socialService.disconnectAsync(SessionService.Username);
                matchmakingService.disconnect();
            }
            catch (EndpointNotFoundException)
            {
                // Servidor no disponible - solo limpiar localmente
            }
            catch (CommunicationException)
            {
                // Error de comunicación - solo limpiar localmente
            }
            catch (TimeoutException)
            {
                // Timeout - solo limpiar localmente
            }
            catch (SocketException)
            {
                // Error de red - solo limpiar localmente
            }
            catch (ObjectDisposedException)
            {
                // Canal ya cerrado - solo limpiar localmente
            }
            finally
            {
                currentMatchService.clearMatchData();
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

        // ELIMINADO: handleHeartbeatDisconnectionAsync
        // ELIMINADO: stopHeartbeatSafeAsync
        // ELIMINADO: forceStopHeartbeatSafe
    }
}