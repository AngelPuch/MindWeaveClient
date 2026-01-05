using System;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Abstractions
{
    /// <summary>
    /// Interface for the heartbeat service that maintains connection health with the server.
    /// The heartbeat runs globally after login, independent of the client's current state
    /// (menu, lobby, game, social, profile, etc.).
    /// </summary>
    public interface IHeartbeatService : IDisposable
    {
        /// <summary>
        /// Event raised when the server terminates the connection due to heartbeat timeout
        /// or other critical issues. The string parameter contains the reason code.
        /// </summary>
        event Action<string> OnConnectionTerminated;

        /// <summary>
        /// Event raised when the heartbeat service detects a connection problem.
        /// This is raised before the server officially terminates the connection.
        /// </summary>
        event Action OnConnectionUnhealthy;

        /// <summary>
        /// Indicates whether the heartbeat service is currently running and connected.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Indicates whether the connection is considered healthy based on recent heartbeat responses.
        /// </summary>
        bool IsConnectionHealthy { get; }

        /// <summary>
        /// Starts the heartbeat service after successful login.
        /// This method should be called immediately after the user logs in.
        /// </summary>
        /// <param name="username">The username of the logged-in user.</param>
        /// <returns>True if the heartbeat was successfully registered with the server.</returns>
        Task<bool> startAsync(string username);

        /// <summary>
        /// Stops the heartbeat service gracefully.
        /// This method should be called during logout or when the application closes.
        /// </summary>
        Task stopAsync();

        /// <summary>
        /// Forces an immediate stop without waiting for graceful shutdown.
        /// Used during critical errors or application crash scenarios.
        /// </summary>
        void forceStop();
    }
}