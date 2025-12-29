using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Windows;

namespace MindWeaveClient.Utilities.Implementations
{
    public class ServiceExceptionHandler : IServiceExceptionHandler
    {
        private readonly IDialogService dialogService;
        private readonly IWindowNavigationService windowNavigationService;
        private readonly Lazy<ISessionCleanupService> sessionCleanupServiceLazy;

        private static bool isHandlingCriticalError;
        private static readonly object lockObject = new object();

        public ServiceExceptionHandler(
            IDialogService dialogService,
            IWindowNavigationService windowNavigationService,
            Lazy<ISessionCleanupService> sessionCleanupServiceLazy)
        {
            this.dialogService = dialogService;
            this.windowNavigationService = windowNavigationService;
            this.sessionCleanupServiceLazy = sessionCleanupServiceLazy;
        }

        public bool handleException(Exception exception, string operationContext = null)
        {
            if (exception == null) return false;

            if (isCriticalConnectionError(exception))
            {
                handleCriticalConnectionError(exception);
                return true;
            }

            if (exception is FaultException faultException)
            {
                handleFaultException(faultException);
                return false;
            }

            string message = getExceptionMessage(exception, operationContext);
            showErrorOnUIThread(message, Lang.ErrorTitle);
            return false;
        }

        public void handleExceptionAsync(Exception exception, string operationContext = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                handleException(exception, operationContext);
            });
        }

        public bool isCriticalConnectionError(Exception exception)
        {
            if (exception == null) return false;

            if (exception is EndpointNotFoundException) return true;
            if (exception is CommunicationObjectFaultedException) return true;
            if (exception is ChannelTerminatedException) return true;
            if (exception is ServerTooBusyException) return true;

            if (exception is CommunicationException commEx)
            {
                if (commEx.InnerException is SocketException) return true;

                if (commEx.Message.Contains("connection") ||
                    commEx.Message.Contains("socket") ||
                    commEx.Message.Contains("refused") ||
                    commEx.Message.Contains("reset"))
                {
                    return true;
                }
            }

            if (exception is TimeoutException) return true;

            if (exception is SocketException) return true;

            if (exception.InnerException != null)
            {
                return isCriticalConnectionError(exception.InnerException);
            }

            return false;
        }

        public void performSoftReset()
        {
            lock (lockObject)
            {
                if (isHandlingCriticalError) return;
                isHandlingCriticalError = true;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        await sessionCleanupServiceLazy.Value.cleanUpSessionAsync();
                        resetApplicationWindows();
                    }
                    finally
                    {
                        lock (lockObject)
                        {
                            isHandlingCriticalError = false;
                        }
                    }
                });
            }
            catch (Exception)
            {
                lock (lockObject)
                {
                    isHandlingCriticalError = false;
                }
            }
        }

        private void handleCriticalConnectionError(Exception exception)
        {
            lock (lockObject)
            {
                if (isHandlingCriticalError) return;
            }

            string message = getCriticalErrorMessage(exception);

            showErrorOnUIThread(message, Lang.ErrorConnectionLostTitle);
            performSoftReset();
        }

        private void handleFaultException(FaultException faultException)
        {
            string message = faultException.Message;

            if (faultException.GetType().IsGenericType)
            {
                var detailProperty = faultException.GetType().GetProperty("Detail");
                if (detailProperty != null)
                {
                    var detail = detailProperty.GetValue(faultException);
                    if (detail != null)
                    {
                        var messageProperty = detail.GetType().GetProperty("Message");
                        if (messageProperty != null)
                        {
                            message = messageProperty.GetValue(detail)?.ToString() ?? message;
                        }
                    }
                }
            }

            showErrorOnUIThread(message, Lang.ErrorTitle);
        }

        private static string getCriticalErrorMessage(Exception exception)
        {
            if (exception is EndpointNotFoundException)
            {
                return Lang.ErrorServiceNotFound;
            }

            if (exception is TimeoutException)
            {
                return Lang.ErrorServerTimeout;
            }

            if (exception is CommunicationObjectFaultedException)
            {
                return Lang.ErrorConnectionLost;
            }

            if (exception is ChannelTerminatedException)
            {
                return Lang.ErrorConnectionTerminated;
            }

            if (exception is ServerTooBusyException)
            {
                return Lang.ErrorServerBusy;
            }

            if (exception is CommunicationException)
            {
                if (exception.InnerException is SocketException socketEx)
                {
                    return getSocketErrorMessage(socketEx);
                }
                return Lang.ErrorConnectionLost;
            }

            if (exception is SocketException socketException)
            {
                return getSocketErrorMessage(socketException);
            }

            return Lang.ErrorConnectionLost;
        }

        private static string getSocketErrorMessage(SocketException socketException)
        {
            switch (socketException.SocketErrorCode)
            {
                case SocketError.ConnectionRefused:
                    return Lang.ErrorConnectionRefused;

                case SocketError.ConnectionReset:
                    return Lang.ErrorConnectionReset;

                case SocketError.HostUnreachable:
                case SocketError.NetworkUnreachable:
                    return Lang.ErrorNetworkUnreachable;

                case SocketError.HostNotFound:
                    return Lang.ErrorHostNotFound;

                case SocketError.TimedOut:
                    return Lang.ErrorServerTimeout;

                default:
                    return Lang.ErrorConnectionLost;
            }
        }

        private static string getExceptionMessage(Exception exception, string operationContext)
        {
            if (exception is SecurityNegotiationException)
            {
                return Lang.ErrorSecurityNegotiation;
            }

            if (exception is MessageSecurityException)
            {
                return Lang.ErrorMessageSecurity;
            }

            if (exception is QuotaExceededException)
            {
                return Lang.ErrorQuotaExceeded;
            }

            if (exception is ProtocolException)
            {
                return Lang.ErrorProtocol;
            }

            if (exception is ActionNotSupportedException)
            {
                return Lang.ErrorActionNotSupported;
            }

            if (exception is InvalidOperationException)
            {
                return Lang.ErrorInvalidOperation;
            }

            if (exception is ArgumentException)
            {
                return Lang.ErrorInvalidData;
            }

            return string.Format(Lang.ErrorGenericOperation, operationContext ?? Lang.GlobalLbUnknown);
        }

        private void showErrorOnUIThread(string message, string title)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                dialogService.showError(message, title);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    dialogService.showError(message, title);
                });
            }
        }

        private void resetApplicationWindows()
        {          
            var loginWindow = getOrCreateLoginWindow();

            if (loginWindow == null) { return; }
            Application.Current.MainWindow = loginWindow;

            closeWindowsExcept(loginWindow);
        }

        private Window getOrCreateLoginWindow()
        {
            var existingLogin = Application.Current.Windows.OfType<View.Authentication.AuthenticationWindow>().FirstOrDefault();

            if (existingLogin != null)
            {
                if (existingLogin.WindowState == WindowState.Minimized)
                {
                    existingLogin.WindowState = WindowState.Normal;
                }
                existingLogin.Activate();
                return existingLogin;
            }

            windowNavigationService.openWindow<View.Authentication.AuthenticationWindow>();

            return Application.Current.Windows.OfType<View.Authentication.AuthenticationWindow>().LastOrDefault();
        }

        private static void closeWindowsExcept(Window windowToKeep)
        {
            var windowsToClose = new System.Collections.Generic.List<Window>();

            foreach (Window window in Application.Current.Windows)
            {
                if (!ReferenceEquals(window, windowToKeep))
                {
                    windowsToClose.Add(window);
                }
            }

            foreach (var window in windowsToClose)
            {
                window.Close();
            }
        }
    }
}