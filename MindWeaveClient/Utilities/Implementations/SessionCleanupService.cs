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
        private readonly IHeartbeatService heartbeatService;

        public SessionCleanupService(
            IAuthenticationService authenticationService,
            ISocialService socialService,
            IMatchmakingService matchmakingService,
            ICurrentMatchService currentMatchService,
            IHeartbeatService heartbeatService)
        {
            this.authenticationService = authenticationService;
            this.socialService = socialService;
            this.matchmakingService = matchmakingService;
            this.currentMatchService = currentMatchService;
            this.heartbeatService = heartbeatService;
        }

        public async Task cleanUpSessionAsync()
        {
            try
            {
                await stopHeartbeatSafeAsync();

                if (!SessionService.IsGuest && !string.IsNullOrEmpty(SessionService.Username))
                {
                    await authenticationService.logoutAsync(SessionService.Username);
                }

                await socialService.disconnectAsync(SessionService.Username);
                matchmakingService.disconnect();
            }
            catch (EndpointNotFoundException)
            {
                forceStopHeartbeatSafe();
            }
            catch (CommunicationException)
            {
                forceStopHeartbeatSafe();
            }
            catch (TimeoutException)
            {
                forceStopHeartbeatSafe();
            }
            catch (SocketException)
            {
                forceStopHeartbeatSafe();
            }
            catch (ObjectDisposedException)
            {
                forceStopHeartbeatSafe();
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

        public async Task handleHeartbeatDisconnectionAsync(string reason)
        {
            System.Diagnostics.Debug.WriteLine($"[CLEANUP] Handling heartbeat disconnection. Reason: {reason}");

            try
            {
                forceStopHeartbeatSafe();

                try
                {
                    await socialService.disconnectAsync(SessionService.Username);
                }
                catch
                {
                    // Ignore
                }

                try
                {
                    matchmakingService.disconnect();
                }
                catch
                {
                    // Ignore
                }
            }
            finally
            {
                currentMatchService.clearMatchData();
                SessionService.clearSession();
            }
        }

        private async Task stopHeartbeatSafeAsync()
        {
            if (heartbeatService == null) return;

            try
            {
                await heartbeatService.stopAsync();
            }
            catch (Exception)
            {
                forceStopHeartbeatSafe();
            }
        }

        private void forceStopHeartbeatSafe()
        {
            try
            {
                heartbeatService?.forceStop();
            }
            catch (Exception)
            {
                // Ignore
            }
        }
    }
}