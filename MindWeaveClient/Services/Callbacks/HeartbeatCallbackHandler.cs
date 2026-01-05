using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Threading;
using MindWeaveClient.HeartbeatService;

namespace MindWeaveClient.Services.Callbacks
{
    /// <summary>
    /// Callback handler for heartbeat service responses from the server.
    /// Receives heartbeat acknowledgments and connection termination notifications.
    /// </summary>
    [CallbackBehavior(
        ConcurrencyMode = ConcurrencyMode.Reentrant,
        UseSynchronizationContext = false)]
    public class HeartbeatCallbackHandler : IHeartbeatManagerCallback
    {
        /// <summary>
        /// Event raised when a heartbeat acknowledgment is received from the server.
        /// Parameters: sequenceNumber, serverTimestamp (Unix milliseconds)
        /// </summary>
        public event Action<long, long> OnHeartbeatAckReceived;

        /// <summary>
        /// Event raised when the server notifies that the connection is being terminated.
        /// Parameter: reason code for the termination
        /// </summary>
        public event Action<string> OnConnectionTerminatingReceived;

        /// <summary>
        /// Called by the server to acknowledge a heartbeat.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number being acknowledged.</param>
        /// <param name="serverTimestamp">The server's timestamp in Unix milliseconds.</param>
        public void heartbeatAck(long sequenceNumber, long serverTimestamp)
        {
            try
            {
                OnHeartbeatAckReceived?.Invoke(sequenceNumber, serverTimestamp);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HEARTBEAT_CALLBACK] Error in heartbeatAck handler: {ex.Message}");
            }
        }

        /// <summary>
        /// Called by the server to notify that the connection is being terminated.
        /// This happens when heartbeat timeout is detected or other critical issues occur.
        /// </summary>
        /// <param name="reason">The reason code for termination (e.g., HeartbeatTimeout, ChannelFaulted).</param>
        public void connectionTerminating(string reason)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[HEARTBEAT_CALLBACK] Connection terminating received. Reason: {reason}");

                // Dispatch to UI thread for any UI-related handling
                if (Application.Current?.Dispatcher != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action(() =>
                        {
                            OnConnectionTerminatingReceived?.Invoke(reason);
                        }));
                }
                else
                {
                    OnConnectionTerminatingReceived?.Invoke(reason);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HEARTBEAT_CALLBACK] Error in connectionTerminating handler: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all event subscriptions to prevent memory leaks.
        /// </summary>
        public void clearSubscriptions()
        {
            OnHeartbeatAckReceived = null;
            OnConnectionTerminatingReceived = null;
        }
    }
}