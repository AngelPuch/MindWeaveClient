using System.Threading.Tasks;

namespace MindWeaveClient.Utilities.Abstractions
{
    /// <summary>
    /// Interface for session cleanup operations.
    /// Handles proper cleanup of all client-side state during logout, disconnection, or connection failures.
    /// </summary>
    public interface ISessionCleanupService
    {
        /// <summary>
        /// Performs a complete session cleanup including heartbeat, authentication, and all services.
        /// Should be called during normal logout.
        /// </summary>
        Task cleanUpSessionAsync();

        /// <summary>
        /// Cleans up when exiting from an active game.
        /// Leaves the game first, then performs full session cleanup.
        /// </summary>
        Task exitGameInProcessAsync();

        /// <summary>
        /// Handles cleanup when a heartbeat connection failure is detected.
        /// Called by the heartbeat service when the connection is lost unexpectedly.
        /// </summary>
        /// <param name="reason">The reason code for the disconnection.</param>
        Task handleHeartbeatDisconnectionAsync(string reason);
    }
}