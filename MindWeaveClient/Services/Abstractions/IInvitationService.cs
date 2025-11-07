namespace MindWeaveClient.Services.Abstractions
{
    public interface IInvitationService
    {
        /// <summary>
        /// Subscribes to global WCF callbacks.
        /// </summary>
        void subscribeToGlobalInvites();

        /// <summary>
        /// Unsubscribes from global WCF callbacks.
        /// </summary>
        void unsubscribeFromGlobalInvites();
    }
}
