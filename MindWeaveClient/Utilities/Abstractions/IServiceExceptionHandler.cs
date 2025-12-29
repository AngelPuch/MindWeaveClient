using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWeaveClient.Utilities.Abstractions
{
    public interface IServiceExceptionHandler
    {
        /// <summary>
        /// Handles exceptions from service calls.
        /// Returns true if the exception was handled and the caller should stop processing.
        /// Returns false if it's a non-critical error that was just displayed to the user.
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="operationContext">Optional context describing the operation that failed</param>
        /// <returns>True if critical error requiring soft reset, false otherwise</returns>
        bool handleException(Exception exception, string operationContext = null);

        /// <summary>
        /// Handles exceptions asynchronously, useful for async/await patterns.
        /// Ensures the exception is handled on the UI thread.
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="operationContext">Optional context describing the operation that failed</param>
        void handleExceptionAsync(Exception exception, string operationContext = null);

        /// <summary>
        /// Determines if the exception is a critical connection error requiring soft reset.
        /// </summary>
        /// <param name="exception">The exception to check</param>
        /// <returns>True if it's a critical connection error</returns>
        bool isCriticalConnectionError(Exception exception);

        /// <summary>
        /// Performs a soft reset: cleans up session and redirects to login.
        /// </summary>
        void performSoftReset();
    }
}
