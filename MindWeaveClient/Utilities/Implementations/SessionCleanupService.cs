using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Utilities.Implementations
{
    /// <summary>
    /// Service responsible for cleaning up all client-side state during logout or disconnection.
    /// Ensures proper cleanup of heartbeat, authentication, social, and matchmaking services.
    /// 
    /// CRITICAL: Heartbeat must be stopped BEFORE logout to prevent server from detecting
    /// a false disconnect while we're in the process of logging out gracefully.
    /// </summary>
    public class SessionCleanupService : ISessionCleanupService
    {
        private readonly IAuthenticationService authenticationService;
        private readonly ISocialService socialService;
        private readonly IMatchmakingService matchmakingService;
        private readonly ICurrentMatchService currentMatchService;
        private readonly IHeartbeatService heartbeatService;

        /// <summary>
        /// Creates a new SessionCleanupService with all required dependencies.
        /// </summary>
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

        /// <summary>
        /// Performs a complete session cleanup including heartbeat, authentication, and all services.
        /// </summary>
        public async Task cleanUpSessionAsync()
        {
            try
            {
                // ╔═══════════════════════════════════════════════════════════════════╗
                // ║ CRITICAL: Stop heartbeat FIRST to prevent server from detecting   ║
                // ║ false disconnect while we're logging out gracefully               ║
                // ╚═══════════════════════════════════════════════════════════════════╝
                await stopHeartbeatSafeAsync();

                // Then proceed with normal logout
                if (!SessionService.IsGuest && !string.IsNullOrEmpty(SessionService.Username))
                {
                    await authenticationService.logoutAsync(SessionService.Username);
                }

                await socialService.disconnectAsync(SessionService.Username);
                matchmakingService.disconnect();
            }
            catch (EndpointNotFoundException)
            {
                // Server not available - just force stop heartbeat
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
                SessionService.clearSession();
            }
        }

        /// <summary>
        /// Cleans up when exiting from an active game.
        /// Leaves the game first, then performs full session cleanup.
        /// </summary>
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

        /// <summary>
        /// Handles cleanup when a heartbeat connection failure is detected.
        /// Called by the heartbeat service when the connection is lost.
        /// 
        /// This method differs from cleanUpSessionAsync in that:
        /// 1. The heartbeat is already stopped (or faulted)
        /// 2. Other services may already be disconnected
        /// 3. We don't try to communicate with the server
        /// </summary>
        public async Task handleHeartbeatDisconnectionAsync(string reason)
        {
            System.Diagnostics.Debug.WriteLine($"[CLEANUP] Handling heartbeat disconnection. Reason: {reason}");

            try
            {
                // Force stop the heartbeat (it may already be stopped)
                forceStopHeartbeatSafe();

                // Disconnect other services silently - they may already be disconnected
                try
                {
                    await socialService.disconnectAsync(SessionService.Username);
                }
                catch { /* Ignore - service may already be disconnected */ }

                try
                {
                    matchmakingService.disconnect();
                }
                catch { /* Ignore - service may already be disconnected */ }
            }
            finally
            {
                SessionService.clearSession();
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PRIVATE METHODS - Heartbeat Cleanup
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Gracefully stops the heartbeat service.
        /// </summary>
        private async Task stopHeartbeatSafeAsync()
        {
            if (heartbeatService == null) return;

            try
            {
                await heartbeatService.stopAsync();
                System.Diagnostics.Debug.WriteLine("[CLEANUP] Heartbeat stopped gracefully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CLEANUP] Error stopping heartbeat gracefully: {ex.Message}");
                forceStopHeartbeatSafe();
            }
        }

        /// <summary>
        /// Forces an immediate stop of the heartbeat service without waiting for graceful shutdown.
        /// </summary>
        private void forceStopHeartbeatSafe()
        {
            try
            {
                heartbeatService?.forceStop();
                System.Diagnostics.Debug.WriteLine("[CLEANUP] Heartbeat force stopped");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CLEANUP] Error force stopping heartbeat: {ex.Message}");
            }
        }
    }
}