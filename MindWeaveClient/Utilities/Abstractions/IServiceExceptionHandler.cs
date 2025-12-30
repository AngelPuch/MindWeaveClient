using System;

namespace MindWeaveClient.Utilities.Abstractions
{
    public interface IServiceExceptionHandler
    {
        /// <summary>
        /// Handles an exception with full UI feedback (shows error dialogs).
        /// Use for user-initiated operations like button clicks.
        /// Returns true if the exception was a critical connection error that triggered a soft reset.
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="operationContext">Optional context describing the operation that failed</param>
        /// <returns>True if it was a critical error, false otherwise</returns>
        bool handleException(Exception exception, string operationContext = null);

        /// <summary>
        /// Handles an exception asynchronously on the UI thread.
        /// </summary>
        void handleExceptionAsync(Exception exception, string operationContext = null);

        /// <summary>
        /// Handles an exception silently - only acts on critical connection errors.
        /// Use for high-frequency operations like piece movements where showing
        /// repeated error messages would be disruptive.
        /// Returns true if it was a critical error that triggered a soft reset.
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <returns>True if it was a critical connection error, false otherwise</returns>
        bool handleExceptionSilent(Exception exception);

        /// <summary>
        /// Checks if an exception represents a critical connection error
        /// (server down, network issues, etc.)
        /// </summary>
        bool isCriticalConnectionError(Exception exception);

        /// <summary>
        /// Checks if an exception represents a network unavailability error
        /// (client has no internet connection)
        /// </summary>
        bool isNetworkUnavailableError(Exception exception);

        /// <summary>
        /// Checks if an exception represents a WCF channel state error
        /// (channel faulted, closed, etc.)
        /// </summary>
        bool isChannelStateError(Exception exception);

        /// <summary>
        /// Performs a soft reset of the application - cleans up session
        /// and navigates back to login screen.
        /// </summary>
        void performSoftReset();
    }
}