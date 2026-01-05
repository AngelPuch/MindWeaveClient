using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Diagnostics;
using System.Windows;

namespace MindWeaveClient.Services.Implementations
{
    /// <summary>
    /// Handles heartbeat connection failures and coordinates with the service exception handler
    /// to perform application-wide error recovery.
    /// 
    /// This class subscribes to heartbeat events and triggers the standard
    /// connection failure recovery flow when the heartbeat detects issues.
    /// </summary>
    public class HeartbeatConnectionHandler : IDisposable
    {
        private readonly IHeartbeatService heartbeatService;
        private readonly IServiceExceptionHandler exceptionHandler;
        private readonly Lazy<ISessionCleanupService> sessionCleanupServiceLazy;
        private readonly IDialogService dialogService;

        private bool isDisposed;
        private bool isHandlingDisconnection;
        private readonly object lockObject = new object();

        /// <summary>
        /// Creates a new HeartbeatConnectionHandler with all required dependencies.
        /// </summary>
        public HeartbeatConnectionHandler(
            IHeartbeatService heartbeatService,
            IServiceExceptionHandler exceptionHandler,
            Lazy<ISessionCleanupService> sessionCleanupServiceLazy,
            IDialogService dialogService)
        {
            this.heartbeatService = heartbeatService ?? throw new ArgumentNullException(nameof(heartbeatService));
            this.exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            this.sessionCleanupServiceLazy = sessionCleanupServiceLazy ?? throw new ArgumentNullException(nameof(sessionCleanupServiceLazy));
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Subscribe to heartbeat events
            subscribeToHeartbeatEvents();
        }

        /// <summary>
        /// Subscribes to all relevant heartbeat service events.
        /// </summary>
        private void subscribeToHeartbeatEvents()
        {
            heartbeatService.OnConnectionTerminated += handleConnectionTerminated;
            heartbeatService.OnConnectionUnhealthy += handleConnectionUnhealthy;
        }

        /// <summary>
        /// Unsubscribes from all heartbeat service events.
        /// </summary>
        private void unsubscribeFromHeartbeatEvents()
        {
            if (heartbeatService != null)
            {
                heartbeatService.OnConnectionTerminated -= handleConnectionTerminated;
                heartbeatService.OnConnectionUnhealthy -= handleConnectionUnhealthy;
            }
        }

        /// <summary>
        /// Handles the connection terminated event from the heartbeat service.
        /// This triggers the full application reset flow.
        /// </summary>
        private void handleConnectionTerminated(string reason)
        {
            lock (lockObject)
            {
                if (isHandlingDisconnection || isDisposed)
                {
                    return;
                }
                isHandlingDisconnection = true;
            }

            Debug.WriteLine($"[HEARTBEAT_HANDLER] Connection terminated. Reason: {reason}");

            try
            {
                // Execute on UI thread
                if (Application.Current?.Dispatcher != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        executeDisconnectionHandling(reason);
                    });
                }
                else
                {
                    executeDisconnectionHandling(reason);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HEARTBEAT_HANDLER] Error handling disconnection: {ex.Message}");
            }
            finally
            {
                lock (lockObject)
                {
                    isHandlingDisconnection = false;
                }
            }
        }

        /// <summary>
        /// Executes the disconnection handling logic on the UI thread.
        /// </summary>
        private void executeDisconnectionHandling(string reason)
        {
            // Show appropriate message based on reason
            string message = getDisconnectionMessage(reason);
            string title = getDisconnectionTitle(reason);

            dialogService.showError(message, title);

            // Use the service exception handler's soft reset mechanism
            // This will clean up the session and navigate back to login
            exceptionHandler.performSoftReset();
        }

        /// <summary>
        /// Handles the connection unhealthy event (warning before disconnect).
        /// Currently just logs - could be extended to show warning UI.
        /// </summary>
        private void handleConnectionUnhealthy()
        {
            Debug.WriteLine("[HEARTBEAT_HANDLER] Connection becoming unhealthy - potential disconnect soon");

            // Could show a subtle warning indicator in the UI
            // For now, just log it
        }

        /// <summary>
        /// Gets a localized message for the disconnection reason.
        /// Server uses: HeartbeatTimeout, ChannelFaulted
        /// Client uses: HEARTBEAT_TIMEOUT_CLIENT, HEARTBEAT_CHANNEL_FAULTED, HEARTBEAT_CHANNEL_CLOSED, HEARTBEAT_SEND_FAILED
        /// </summary>
        private static string getDisconnectionMessage(string reason)
        {
            switch (reason)
            {
                // Server-side reason codes
                case "HeartbeatTimeout":
                case "HEARTBEAT_TIMEOUT":
                case "HEARTBEAT_TIMEOUT_CLIENT":
                    return Lang.ErrorServerTimeout ?? "Connection lost due to timeout. Please log in again.";

                // Channel issues
                case "ChannelFaulted":
                case "HEARTBEAT_CHANNEL_FAULTED":
                case "HEARTBEAT_CHANNEL_CLOSED":
                    return Lang.ErrorConnectionLost ?? "Connection to server was lost. Please log in again.";

                // Send failures
                case "HEARTBEAT_SEND_FAILED":
                    return Lang.ErrorCommunication ?? "Unable to communicate with server. Please check your connection and log in again.";

                default:
                    return Lang.ErrorServerWentDown ?? "Server connection was lost. Please log in again.";
            }
        }

        /// <summary>
        /// Gets a localized title for the disconnection reason.
        /// </summary>
        private static string getDisconnectionTitle(string reason)
        {
            return Lang.ErrorConnectionLostTitle ?? "Connection Lost";
        }

        /// <summary>
        /// Disposes the handler and unsubscribes from events.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;
            unsubscribeFromHeartbeatEvents();

            Debug.WriteLine("[HEARTBEAT_HANDLER] Disposed");
        }
    }
}