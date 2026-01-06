using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Threading;
using MindWeaveClient.HeartbeatService;

namespace MindWeaveClient.Services.Callbacks
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class HeartbeatCallbackHandler : IHeartbeatManagerCallback
    {
        public event Action<long, long> OnHeartbeatAckReceived;
        public event Action<string> OnConnectionTerminatingReceived;

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

        public void connectionTerminating(string reason)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[HEARTBEAT_CALLBACK] Connection terminating received. Reason: {reason}");

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
    }
}