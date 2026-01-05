using MindWeaveClient.HeartbeatService;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    /// <summary>
    /// Heartbeat service implementation that maintains connection health with the server.
    /// Runs independently in the background after login, sending periodic heartbeats
    /// and monitoring for server acknowledgments.
    /// 
    /// Key features:
    /// - Timer-based heartbeat sending every 500ms (configurable by server)
    /// - Connection health monitoring with timeout detection
    /// - Automatic detection of channel faults and connection failures
    /// - Event-based notification to application layer
    /// </summary>
    public class HeartbeatService : IHeartbeatService
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // CONSTANTS - Match server configuration for LAN gaming
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>Default heartbeat interval in milliseconds (matches server expectation).</summary>
        private const int DEFAULT_HEARTBEAT_INTERVAL_MS = 500;

        /// <summary>Default timeout before considering connection unhealthy.</summary>
        private const int DEFAULT_HEARTBEAT_TIMEOUT_MS = 2500;

        /// <summary>Maximum consecutive missed ACKs before considering connection dead.</summary>
        private const int MAX_MISSED_ACKS = 5;

        /// <summary>Delay before attempting reconnection after failure.</summary>
        private const int RECONNECT_DELAY_MS = 1000;

        /// <summary>Maximum reconnection attempts before giving up.</summary>
        private const int MAX_RECONNECT_ATTEMPTS = 3;

        /// <summary>Grace period for stopping the heartbeat timer.</summary>
        private const int STOP_GRACE_PERIOD_MS = 1000;

        // ═══════════════════════════════════════════════════════════════════════════
        // PRIVATE FIELDS
        // ═══════════════════════════════════════════════════════════════════════════

        private readonly HeartbeatCallbackHandler callbackHandler;
        private readonly object lockObject = new object();

        private HeartbeatManagerClient proxy;
        private Timer heartbeatTimer;
        private string currentUsername;

        private int heartbeatIntervalMs = DEFAULT_HEARTBEAT_INTERVAL_MS;
        private int heartbeatTimeoutMs = DEFAULT_HEARTBEAT_TIMEOUT_MS;

        private long currentSequenceNumber;
        private long lastAcknowledgedSequence;
        private long lastAckReceivedTimestamp;
        private int consecutiveMissedAcks;

        private volatile bool isRunning;
        private volatile bool isDisposed;
        private volatile bool isStopping;

        // ═══════════════════════════════════════════════════════════════════════════
        // EVENTS
        // ═══════════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        public event Action<string> OnConnectionTerminated;

        /// <inheritdoc/>
        public event Action OnConnectionUnhealthy;

        // ═══════════════════════════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        public bool IsRunning => isRunning && !isDisposed;

        /// <inheritdoc/>
        public bool IsConnectionHealthy
        {
            get
            {
                if (!isRunning || isDisposed) return false;

                long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                long timeSinceLastAck = now - lastAckReceivedTimestamp;
                return timeSinceLastAck < heartbeatTimeoutMs
                       && consecutiveMissedAcks < MAX_MISSED_ACKS;
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Creates a new HeartbeatService instance.
        /// </summary>
        /// <param name="callbackHandler">The callback handler for server responses.</param>
        public HeartbeatService(HeartbeatCallbackHandler callbackHandler)
        {
            this.callbackHandler = callbackHandler ?? throw new ArgumentNullException(nameof(callbackHandler));

            // Subscribe to callback events
            this.callbackHandler.OnHeartbeatAckReceived += handleHeartbeatAck;
            this.callbackHandler.OnConnectionTerminatingReceived += handleConnectionTerminating;
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PUBLIC METHODS
        // ═══════════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        public async Task<bool> startAsync(string username)
        {
            if (isDisposed)
            {
                Debug.WriteLine("[HEARTBEAT] Cannot start - service is disposed");
                return false;
            }

            if (string.IsNullOrEmpty(username))
            {
                Debug.WriteLine("[HEARTBEAT] Cannot start - username is null or empty");
                return false;
            }

            if (isRunning)
            {
                Debug.WriteLine("[HEARTBEAT] Already running, stopping first...");
                await stopAsync();
            }

            lock (lockObject)
            {
                currentUsername = username;
                currentSequenceNumber = 0;
                lastAcknowledgedSequence = 0;
                lastAckReceivedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                consecutiveMissedAcks = 0;
                isStopping = false;
            }

            try
            {
                // Create the WCF proxy with callback
                var instanceContext = new InstanceContext(callbackHandler);
                proxy = new HeartbeatManagerClient(instanceContext);

                // Register with the server
                Debug.WriteLine($"[HEARTBEAT] Registering heartbeat for user: {username}");
                HeartbeatRegistrationResult result = await Task.Run(() =>
                    proxy.registerForHeartbeat(username));

                if (result == null || !result.Success)
                {
                    string errorCode = result?.MessageCode ?? "UNKNOWN_ERROR";
                    Debug.WriteLine($"[HEARTBEAT] Registration failed: {errorCode}");
                    abortProxySafe();
                    return false;
                }

                // Use server-provided intervals if available
                if (result.HeartbeatIntervalMs > 0)
                {
                    heartbeatIntervalMs = result.HeartbeatIntervalMs;
                }
                if (result.TimeoutMs > 0)  // NOTE: Server uses TimeoutMs, not HeartbeatTimeoutMs
                {
                    heartbeatTimeoutMs = result.TimeoutMs;
                }

                Debug.WriteLine($"[HEARTBEAT] Registered successfully. Interval: {heartbeatIntervalMs}ms, Timeout: {heartbeatTimeoutMs}ms");

                // Subscribe to channel faulted events
                subscribeToChannelEvents();

                // Start the heartbeat timer
                startHeartbeatTimer();

                isRunning = true;
                return true;
            }
            catch (EndpointNotFoundException ex)
            {
                Debug.WriteLine($"[HEARTBEAT] Server not found: {ex.Message}");
                abortProxySafe();
                return false;
            }
            catch (CommunicationException ex)
            {
                Debug.WriteLine($"[HEARTBEAT] Communication error during registration: {ex.Message}");
                abortProxySafe();
                return false;
            }
            catch (TimeoutException ex)
            {
                Debug.WriteLine($"[HEARTBEAT] Timeout during registration: {ex.Message}");
                abortProxySafe();
                return false;
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"[HEARTBEAT] Socket error during registration: {ex.Message}");
                abortProxySafe();
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HEARTBEAT] Unexpected error during registration: {ex.Message}");
                abortProxySafe();
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task stopAsync()
        {
            if (!isRunning || isStopping)
            {
                return;
            }

            isStopping = true;
            Debug.WriteLine("[HEARTBEAT] Stopping heartbeat service...");

            try
            {
                // Stop the timer first
                stopHeartbeatTimer();

                // Unregister from server
                if (proxy != null && !string.IsNullOrEmpty(currentUsername))
                {
                    try
                    {
                        if (proxy.State == CommunicationState.Opened)
                        {
                            await Task.Run(() => proxy.unregisterHeartbeat(currentUsername));
                            Debug.WriteLine("[HEARTBEAT] Unregistered from server");
                        }
                    }
                    catch (CommunicationException)
                    {
                        // Expected if channel is already faulted
                    }
                    catch (TimeoutException)
                    {
                        // Expected if server is unresponsive
                    }
                }

                // Close or abort the proxy
                closeProxySafe();
            }
            finally
            {
                lock (lockObject)
                {
                    isRunning = false;
                    currentUsername = null;
                    proxy = null;
                    isStopping = false;
                }
                Debug.WriteLine("[HEARTBEAT] Stopped");
            }
        }

        /// <inheritdoc/>
        public void forceStop()
        {
            Debug.WriteLine("[HEARTBEAT] Force stopping...");

            isStopping = true;
            stopHeartbeatTimer();
            abortProxySafe();

            lock (lockObject)
            {
                isRunning = false;
                currentUsername = null;
                proxy = null;
                isStopping = false;
            }

            Debug.WriteLine("[HEARTBEAT] Force stopped");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;
            forceStop();

            // Unsubscribe from callback events
            if (callbackHandler != null)
            {
                callbackHandler.OnHeartbeatAckReceived -= handleHeartbeatAck;
                callbackHandler.OnConnectionTerminatingReceived -= handleConnectionTerminating;
            }

            Debug.WriteLine("[HEARTBEAT] Disposed");
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PRIVATE METHODS - Timer Management
        // ═══════════════════════════════════════════════════════════════════════════

        private void startHeartbeatTimer()
        {
            heartbeatTimer = new Timer(
                onHeartbeatTimerElapsed,
                null,
                heartbeatIntervalMs,
                heartbeatIntervalMs);

            Debug.WriteLine($"[HEARTBEAT] Timer started with interval {heartbeatIntervalMs}ms");
        }

        private void stopHeartbeatTimer()
        {
            if (heartbeatTimer != null)
            {
                try
                {
                    heartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    heartbeatTimer.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // Already disposed
                }
                finally
                {
                    heartbeatTimer = null;
                }
            }
        }

        private void onHeartbeatTimerElapsed(object state)
        {
            if (!isRunning || isStopping || isDisposed)
            {
                return;
            }

            // Check connection health before sending
            checkConnectionHealth();

            if (!isRunning) return;

            // Send heartbeat
            sendHeartbeat();
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PRIVATE METHODS - Heartbeat Sending
        // ═══════════════════════════════════════════════════════════════════════════

        private void sendHeartbeat()
        {
            if (proxy == null || proxy.State != CommunicationState.Opened)
            {
                Debug.WriteLine("[HEARTBEAT] Cannot send - proxy not in opened state");
                handleChannelFaulted();
                return;
            }

            try
            {
                long sequenceNumber;
                long clientTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                lock (lockObject)
                {
                    sequenceNumber = ++currentSequenceNumber;
                }

                // One-way call - doesn't wait for response
                proxy.sendHeartbeat(currentUsername, sequenceNumber, clientTimestamp);

                Debug.WriteLine($"[HEARTBEAT] Sent seq={sequenceNumber}");
            }
            catch (CommunicationObjectFaultedException)
            {
                Debug.WriteLine("[HEARTBEAT] Channel faulted while sending");
                handleChannelFaulted();
            }
            catch (CommunicationException ex)
            {
                Debug.WriteLine($"[HEARTBEAT] Communication error while sending: {ex.Message}");
                incrementMissedAcks();
            }
            catch (TimeoutException)
            {
                Debug.WriteLine("[HEARTBEAT] Timeout while sending");
                incrementMissedAcks();
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("[HEARTBEAT] Proxy disposed while sending");
                handleChannelFaulted();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HEARTBEAT] Unexpected error while sending: {ex.Message}");
                incrementMissedAcks();
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PRIVATE METHODS - Health Monitoring
        // ═══════════════════════════════════════════════════════════════════════════

        private void checkConnectionHealth()
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long timeSinceLastAck = now - lastAckReceivedTimestamp;

            if (timeSinceLastAck > heartbeatTimeoutMs)
            {
                Debug.WriteLine($"[HEARTBEAT] Connection appears unhealthy. Time since last ACK: {timeSinceLastAck}ms");

                lock (lockObject)
                {
                    consecutiveMissedAcks++;
                }

                if (consecutiveMissedAcks >= MAX_MISSED_ACKS)
                {
                    Debug.WriteLine($"[HEARTBEAT] Max missed ACKs reached ({MAX_MISSED_ACKS}). Triggering connection failure.");
                    handleConnectionFailure("HEARTBEAT_TIMEOUT_CLIENT");
                }
                else
                {
                    // Notify listeners that connection is becoming unhealthy
                    OnConnectionUnhealthy?.Invoke();
                }
            }
        }

        private void incrementMissedAcks()
        {
            lock (lockObject)
            {
                consecutiveMissedAcks++;
            }

            if (consecutiveMissedAcks >= MAX_MISSED_ACKS)
            {
                handleConnectionFailure("HEARTBEAT_SEND_FAILED");
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PRIVATE METHODS - Callback Handlers
        // ═══════════════════════════════════════════════════════════════════════════

        private void handleHeartbeatAck(long sequenceNumber, long serverTimestamp)
        {
            if (!isRunning || isDisposed) return;

            lock (lockObject)
            {
                // Only update if this is a newer sequence number
                if (sequenceNumber > lastAcknowledgedSequence)
                {
                    lastAcknowledgedSequence = sequenceNumber;
                    lastAckReceivedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    consecutiveMissedAcks = 0;

                    Debug.WriteLine($"[HEARTBEAT] ACK received seq={sequenceNumber}, server time={serverTimestamp}");
                }
                else if (sequenceNumber == lastAcknowledgedSequence)
                {
                    // Duplicate ACK, just update the time
                    lastAckReceivedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    Debug.WriteLine($"[HEARTBEAT] Duplicate ACK for seq={sequenceNumber}");
                }
                else
                {
                    // Old ACK (sequence regression) - might indicate network issues
                    Debug.WriteLine($"[HEARTBEAT] Old ACK received seq={sequenceNumber}, expected > {lastAcknowledgedSequence}");
                }
            }
        }

        private void handleConnectionTerminating(string reason)
        {
            Debug.WriteLine($"[HEARTBEAT] Server terminating connection. Reason: {reason}");

            // Stop heartbeat immediately
            forceStop();

            // Notify listeners
            OnConnectionTerminated?.Invoke(reason);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PRIVATE METHODS - Channel Management
        // ═══════════════════════════════════════════════════════════════════════════

        private void subscribeToChannelEvents()
        {
            if (proxy == null) return;

            try
            {
                var commObject = proxy as ICommunicationObject;
                if (commObject != null)
                {
                    commObject.Faulted += onChannelFaulted;
                    commObject.Closed += onChannelClosed;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HEARTBEAT] Error subscribing to channel events: {ex.Message}");
            }
        }

        private void unsubscribeFromChannelEvents()
        {
            if (proxy == null) return;

            try
            {
                var commObject = proxy as ICommunicationObject;
                if (commObject != null)
                {
                    commObject.Faulted -= onChannelFaulted;
                    commObject.Closed -= onChannelClosed;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HEARTBEAT] Error unsubscribing from channel events: {ex.Message}");
            }
        }

        private void onChannelFaulted(object sender, EventArgs e)
        {
            Debug.WriteLine("[HEARTBEAT] Channel faulted event received");
            handleChannelFaulted();
        }

        private void onChannelClosed(object sender, EventArgs e)
        {
            Debug.WriteLine("[HEARTBEAT] Channel closed event received");

            if (isRunning && !isStopping)
            {
                handleConnectionFailure("HEARTBEAT_CHANNEL_CLOSED");
            }
        }

        private void handleChannelFaulted()
        {
            if (!isRunning || isStopping) return;

            handleConnectionFailure("HEARTBEAT_CHANNEL_FAULTED");
        }

        private void handleConnectionFailure(string reason)
        {
            if (!isRunning) return;

            Debug.WriteLine($"[HEARTBEAT] Connection failure detected. Reason: {reason}");

            // Stop the heartbeat
            forceStop();

            // Notify listeners
            OnConnectionTerminated?.Invoke(reason);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PRIVATE METHODS - Proxy Cleanup
        // ═══════════════════════════════════════════════════════════════════════════

        private void closeProxySafe()
        {
            if (proxy == null) return;

            unsubscribeFromChannelEvents();

            try
            {
                if (proxy.State == CommunicationState.Opened)
                {
                    proxy.Close();
                }
                else if (proxy.State != CommunicationState.Closed)
                {
                    proxy.Abort();
                }
            }
            catch (CommunicationException)
            {
                abortProxySafe();
            }
            catch (TimeoutException)
            {
                abortProxySafe();
            }
            catch (Exception)
            {
                abortProxySafe();
            }
        }

        private void abortProxySafe()
        {
            if (proxy == null) return;

            unsubscribeFromChannelEvents();

            try
            {
                proxy.Abort();
            }
            catch
            {
                // Ignore all errors during abort
            }
        }
    }
}