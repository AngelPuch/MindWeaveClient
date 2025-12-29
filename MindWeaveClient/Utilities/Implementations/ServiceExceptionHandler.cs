using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Windows;

namespace MindWeaveClient.Utilities.Implementations
{
    public class ServiceExceptionHandler : IServiceExceptionHandler
    {
        private const int WSAENETDOWN = 10050;        
        private const int WSAENETUNREACH = 10051;     
        private const int WSAENETRESET = 10052;       
        private const int WSAECONNABORTED = 10053;
        private const int WSAENOTCONN = 10057;        
        private const int WSAEHOSTDOWN = 10064;       
        private const int WSAEHOSTUNREACH = 10065;    
        private const int WSASYSNOTREADY = 10091;     
        private const int WSANOTINITIALISED = 10093;  

        private const string KEYWORD_CONNECTION = "connection";
        private const string KEYWORD_SOCKET = "socket";
        private const string KEYWORD_REFUSED = "refused";
        private const string KEYWORD_RESET = "reset";
        private const string KEYWORD_NETWORK = "network";
        private const string KEYWORD_UNREACHABLE = "unreachable";

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
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this.windowNavigationService = windowNavigationService ?? throw new ArgumentNullException(nameof(windowNavigationService));
            this.sessionCleanupServiceLazy = sessionCleanupServiceLazy ?? throw new ArgumentNullException(nameof(sessionCleanupServiceLazy));
        }

        public bool handleException(Exception exception, string operationContext = null)
        {
            if (exception == null) return false;

            if (isNetworkUnavailableError(exception))
            {
                handleNetworkUnavailableError(exception);
                return true;
            }

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
                if (commEx.InnerException is WebException) return true;

                string message = commEx.Message.ToLowerInvariant();
                if (message.Contains(KEYWORD_CONNECTION) ||
                    message.Contains(KEYWORD_SOCKET) ||
                    message.Contains(KEYWORD_REFUSED) ||
                    message.Contains(KEYWORD_RESET))
                {
                    return true;
                }
            }

            if (exception is TimeoutException) return true;
            if (exception is SocketException) return true;
            if (exception is WebException) return true;

            if (exception.InnerException != null)
            {
                return isCriticalConnectionError(exception.InnerException);
            }

            return false;
        }

        public bool isNetworkUnavailableError(Exception exception)
        {
            if (exception == null) return false;

            if (exception is SocketException socketEx)
            {
                if (isNetworkDownSocketError(socketEx.SocketErrorCode) ||
                    isNetworkDownErrorCode(socketEx.ErrorCode))
                {
                    return true;
                }
            }

            if (exception is WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.NameResolutionFailure ||
                    webEx.Status == WebExceptionStatus.ProxyNameResolutionFailure ||
                    webEx.Status == WebExceptionStatus.ConnectFailure)
                {
                    if (!isNetworkAvailable())
                    {
                        return true;
                    }
                }
            }

            if (exception is CommunicationException commEx)
            {
                string message = commEx.Message.ToLowerInvariant();
                if (message.Contains(KEYWORD_NETWORK) && message.Contains(KEYWORD_UNREACHABLE))
                {
                    return true;
                }

                if (commEx.InnerException is SocketException innerSocket)
                {
                    if (isNetworkDownSocketError(innerSocket.SocketErrorCode) ||
                        isNetworkDownErrorCode(innerSocket.ErrorCode))
                    {
                        return true;
                    }
                }

                if (commEx.InnerException is WebException innerWeb)
                {
                    if (innerWeb.Status == WebExceptionStatus.NameResolutionFailure ||
                        innerWeb.Status == WebExceptionStatus.ConnectFailure)
                    {
                        if (!isNetworkAvailable())
                        {
                            return true;
                        }
                    }
                }
            }

            if (exception is EndpointNotFoundException)
            {
                if (!isNetworkAvailable())
                {
                    return true;
                }
            }

            if (exception.InnerException != null)
            {
                return isNetworkUnavailableError(exception.InnerException);
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

        private static bool isNetworkDownSocketError(SocketError errorCode)
        {
            switch (errorCode)
            {
                case SocketError.NetworkDown:
                case SocketError.NetworkUnreachable:
                case SocketError.NetworkReset:
                case SocketError.ConnectionAborted:
                case SocketError.NotConnected:
                case SocketError.HostDown:
                case SocketError.HostUnreachable:
                case SocketError.SystemNotReady:
                case SocketError.NotInitialized:
                    return true;
                default:
                    return false;
            }
        }

        private static bool isNetworkDownErrorCode(int errorCode)
        {
            switch (errorCode)
            {
                case WSAENETDOWN:
                case WSAENETUNREACH:
                case WSAENETRESET:
                case WSAECONNABORTED:
                case WSAENOTCONN:
                case WSAEHOSTDOWN:
                case WSAEHOSTUNREACH:
                case WSASYSNOTREADY:
                case WSANOTINITIALISED:
                    return true;
                default:
                    return false;
            }
        }

        private static bool isNetworkAvailable()
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    return false;
                }

                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var netInterface in interfaces)
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                        netInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                    {
                        continue;
                    }

                    if (netInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        var ipProperties = netInterface.GetIPProperties();
                        if (ipProperties.UnicastAddresses.Count > 0)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch
            {
                return true;
            }
        }


        private void handleNetworkUnavailableError(Exception exception)
        {
            lock (lockObject)
            {
                if (isHandlingCriticalError) return;
            }

            string message = getNetworkUnavailableMessage(exception);
            showErrorOnUIThread(message, Lang.ErrorNoInternetTitle);
            performSoftReset();
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

        private static string getNetworkUnavailableMessage(Exception exception)
        {
            SocketException socketEx = findSocketException(exception);
            if (socketEx != null)
            {
                switch (socketEx.SocketErrorCode)
                {
                    case SocketError.NetworkDown:
                        return Lang.ErrorNetworkDown;

                    case SocketError.NetworkUnreachable:
                    case SocketError.HostUnreachable:
                        return Lang.ErrorNetworkUnreachable;

                    case SocketError.NotConnected:
                        return Lang.ErrorNotConnectedToNetwork;

                    case SocketError.HostDown:
                        return Lang.ErrorHostDown;

                    case SocketError.SystemNotReady:
                    case SocketError.NotInitialized:
                        return Lang.ErrorNetworkNotReady;
                }
            }

            WebException webEx = findWebException(exception);
            if (webEx != null)
            {
                switch (webEx.Status)
                {
                    case WebExceptionStatus.NameResolutionFailure:
                        return Lang.ErrorDnsResolutionFailed;

                    case WebExceptionStatus.ConnectFailure:
                        return Lang.ErrorCannotConnectToNetwork;
                }
            }

            return Lang.ErrorNoInternetConnection;
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

                case SocketError.NetworkDown:
                    return Lang.ErrorNetworkDown;

                case SocketError.NotConnected:
                    return Lang.ErrorNotConnectedToNetwork;

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

        private static SocketException findSocketException(Exception exception)
        {
            if (exception is SocketException socketEx)
            {
                return socketEx;
            }

            if (exception.InnerException != null)
            {
                return findSocketException(exception.InnerException);
            }

            return null;
        }

        private static WebException findWebException(Exception exception)
        {
            if (exception is WebException webEx)
            {
                return webEx;
            }

            if (exception.InnerException != null)
            {
                return findWebException(exception.InnerException);
            }

            return null;
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

            if (loginWindow == null) return;

            Application.Current.MainWindow = loginWindow;
            closeWindowsExcept(loginWindow);
        }

        private Window getOrCreateLoginWindow()
        {
            var existingLogin = Application.Current.Windows
                .OfType<View.Authentication.AuthenticationWindow>()
                .FirstOrDefault();

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

            return Application.Current.Windows
                .OfType<View.Authentication.AuthenticationWindow>()
                .LastOrDefault();
        }

        private static void closeWindowsExcept(Window windowToKeep)
        {
            var windowsToClose = Application.Current.Windows
                .Cast<Window>()
                .Where(w => !ReferenceEquals(w, windowToKeep))
                .ToList();

            foreach (var window in windowsToClose)
            {
                window.Close();
            }
        }
    }
}