using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Windows;

namespace MindWeaveClient.Services.Implementations
{
    public class HeartbeatConnectionHandler : IDisposable
    {
        private readonly IHeartbeatService heartbeatService;
        private readonly IServiceExceptionHandler exceptionHandler;
        private readonly IDialogService dialogService;

        private bool isDisposed;
        private bool isHandlingDisconnection;
        private readonly object lockObject = new object();

        public HeartbeatConnectionHandler(
            IHeartbeatService heartbeatService,
            IServiceExceptionHandler exceptionHandler,
            Lazy<ISessionCleanupService> sessionCleanupServiceLazy,
            IDialogService dialogService)
        {
            this.heartbeatService = heartbeatService ?? throw new ArgumentNullException(nameof(heartbeatService));
            this.exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            subscribeToHeartbeatEvents();
        }

        private void subscribeToHeartbeatEvents()
        {
            heartbeatService.OnConnectionTerminated += handleConnectionTerminated;
        }

        private void unsubscribeFromHeartbeatEvents()
        {
            if (heartbeatService != null)
            {
                heartbeatService.OnConnectionTerminated -= handleConnectionTerminated;
            }
        }

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

            try
            {
                if (Application.Current?.Dispatcher != null)
                {
                    Application.Current.Dispatcher.Invoke(() => { executeDisconnectionHandling(reason); });
                }
                else
                {
                    executeDisconnectionHandling(reason);
                }
            }
            catch (Exception ex)
            {
                //ignored
            }
            finally
            {
                lock (lockObject)
                {
                    isHandlingDisconnection = false;
                }
            }
        }

        private void executeDisconnectionHandling(string reason)
        {
            string message = getDisconnectionMessage(reason);
            string title = getDisconnectionTitle();

            dialogService.showError(message, title);
            exceptionHandler.performSoftReset();
        }

        private static string getDisconnectionMessage(string reason)
        {
            switch (reason)
            {
                case "HeartbeatTimeout":
                case "HEARTBEAT_TIMEOUT":
                case "HEARTBEAT_TIMEOUT_CLIENT":
                    return Lang.ErrorServerTimeout;

                case "ChannelFaulted":
                case "HEARTBEAT_CHANNEL_FAULTED":
                case "HEARTBEAT_CHANNEL_CLOSED":
                    return Lang.ErrorConnectionLost;

                case "HEARTBEAT_SEND_FAILED":
                    return Lang.ErrorCommunication;

                default:
                    return Lang.ErrorServerWentDown;
            }
        }

        private static string getDisconnectionTitle()
        {
            return Lang.ErrorConnectionLostTitle;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                unsubscribeFromHeartbeatEvents();
            }

            isDisposed = true;
        }
    }
}