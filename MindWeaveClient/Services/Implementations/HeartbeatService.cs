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
    public class HeartbeatService : IHeartbeatService
    {
        private const int DEFAULT_HEARTBEAT_INTERVAL_MS = 500;
        private const int DEFAULT_HEARTBEAT_TIMEOUT_MS = 2500;
        private const int MAX_MISSED_ACKS = 5;

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

        public event Action<string> OnConnectionTerminated;
        public event Action OnConnectionUnhealthy;
        public bool IsRunning => isRunning && !isDisposed;

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

        public HeartbeatService(HeartbeatCallbackHandler callbackHandler)
        {
            this.callbackHandler = callbackHandler ?? throw new ArgumentNullException(nameof(callbackHandler));
            this.callbackHandler.OnHeartbeatAckReceived += handleHeartbeatAck;
            this.callbackHandler.OnConnectionTerminatingReceived += handleConnectionTerminating;
        }

        public async Task<bool> startAsync(string username)
        {
            if (isDisposed)
            {
                return false;
            }

            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            if (isRunning)
            {
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
                var instanceContext = new InstanceContext(callbackHandler);
                proxy = new HeartbeatManagerClient(instanceContext);

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

                if (result.HeartbeatIntervalMs > 0)
                {
                    heartbeatIntervalMs = result.HeartbeatIntervalMs;
                }
                if (result.TimeoutMs > 0) 
                {
                    heartbeatTimeoutMs = result.TimeoutMs;
                }

                Debug.WriteLine($"[HEARTBEAT] Registered successfully. Interval: {heartbeatIntervalMs}ms, Timeout: {heartbeatTimeoutMs}ms");

                subscribeToChannelEvents();

                startHeartbeatTimer();

                isRunning = true;
                return true;
            }
            catch (EndpointNotFoundException)
            {
                abortProxySafe();
                return false;
            }
            catch (CommunicationException)
            {
                abortProxySafe();
                return false;
            }
            catch (TimeoutException)
            {
                abortProxySafe();
                return false;
            }
            catch (SocketException)
            {
                abortProxySafe();
                return false;
            }
            catch (Exception)
            {
                abortProxySafe();
                return false;
            }
        }

        public async Task stopAsync()
        {
            if (!isRunning || isStopping)
            {
                return;
            }

            isStopping = true;

            try
            {
                stopHeartbeatTimer();

                if (proxy != null && !string.IsNullOrEmpty(currentUsername))
                {
                    try
                    {
                        if (proxy.State == CommunicationState.Opened)
                        {
                            await Task.Run(() => proxy.unregisterHeartbeat(currentUsername));
                        }
                    }
                    catch (CommunicationException)
                    {
                        // ignored
                    }
                    catch (TimeoutException)
                    {
                        // ignored
                    }
                }

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
            }
        }

        public void forceStop()
        {
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
        }

        public void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;
            forceStop();

            if (callbackHandler != null)
            {
                callbackHandler.OnHeartbeatAckReceived -= handleHeartbeatAck;
                callbackHandler.OnConnectionTerminatingReceived -= handleConnectionTerminating;
            }
        }

        private void startHeartbeatTimer()
        {
            heartbeatTimer = new Timer(
                onHeartbeatTimerElapsed,
                null,
                heartbeatIntervalMs,
                heartbeatIntervalMs);
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
                    // ignored
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

            checkConnectionHealth();

            if (!isRunning) return;

            sendHeartbeat();
        }


        private void sendHeartbeat()
        {
            if (proxy == null || proxy.State != CommunicationState.Opened)
            {
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

                proxy.sendHeartbeat(currentUsername, sequenceNumber, clientTimestamp);

            }
            catch (CommunicationObjectFaultedException)
            {
                handleChannelFaulted();
            }
            catch (CommunicationException)
            {
                incrementMissedAcks();
            }
            catch (TimeoutException)
            {
                incrementMissedAcks();
            }
            catch (ObjectDisposedException)
            {
                handleChannelFaulted();
            }
            catch (Exception)
            {
                incrementMissedAcks();
            }
        }

        private void checkConnectionHealth()
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long timeSinceLastAck = now - lastAckReceivedTimestamp;

            if (timeSinceLastAck > heartbeatTimeoutMs)
            {

                lock (lockObject)
                {
                    consecutiveMissedAcks++;
                }

                if (consecutiveMissedAcks >= MAX_MISSED_ACKS)
                {
                    handleConnectionFailure("HEARTBEAT_TIMEOUT_CLIENT");
                }
                else
                {
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

        private void handleHeartbeatAck(long sequenceNumber, long serverTimestamp)
        {
            if (!isRunning || isDisposed) return;

            lock (lockObject)
            {
                if (sequenceNumber > lastAcknowledgedSequence)
                {
                    lastAcknowledgedSequence = sequenceNumber;
                    lastAckReceivedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    consecutiveMissedAcks = 0;
                }
                else if (sequenceNumber == lastAcknowledgedSequence)
                {
                    lastAckReceivedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
            }
        }

        private void handleConnectionTerminating(string reason)
        {
            forceStop();
            OnConnectionTerminated?.Invoke(reason);
        }

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
            catch (Exception)
            {
                //ignored
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

            forceStop();

            OnConnectionTerminated?.Invoke(reason);
        }

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
                // Ignored
            }
        }
    }
}