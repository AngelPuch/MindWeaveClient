using System;
using System.ServiceModel;
using MindWeaveClient.SocialManagerService;
using System.Threading.Tasks;

namespace MindWeaveClient.Services
{
    public sealed class SocialServiceClientManager
    {
        private static readonly Lazy<SocialServiceClientManager> lazy =
            new Lazy<SocialServiceClientManager>(() => new SocialServiceClientManager());

        public static SocialServiceClientManager instance
        {
            get { return lazy.Value; }
        }

        public SocialManagerClient proxy { get; private set; }
        public SocialCallbackHandler callbackHandler { get; private set; }
        private InstanceContext site;
        private string connectedUsername;

        private SocialServiceClientManager()
        {
        }

        public bool Connect(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Social Service Connect Error: Username is required.");
                return false;
            }

            try
            {
                if (proxy != null && proxy.State == CommunicationState.Opened && connectedUsername == username)
                {
                    Console.WriteLine($"Social Service already connected for user {username}.");
                    return true;
                }

                if (proxy != null)
                {
                    Console.WriteLine(
                        $"Social Service exists but state is {proxy.State} or user mismatch. Disconnecting before reconnecting.");
                    Disconnect();
                }


                Console.WriteLine($"Attempting to connect Social Service for user {username}...");
                callbackHandler = new SocialCallbackHandler();
                site = new InstanceContext(callbackHandler);
                proxy = new SocialManagerClient(site, "NetTcpBinding_ISocialManager");

                proxy.Open();
                Console.WriteLine("Social Service WCF Channel Opened.");

                connectedUsername = username;
                Task.Run(async () =>
                {
                    try
                    {
                        await proxy.connectAsync(username);
                        Console.WriteLine($"Social Service connectAsync('{username}') called successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error calling connectAsync for {username}: {ex.Message}");
                        Disconnect();
                    }
                });


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting Social Service for {username}: {ex.Message}");
                Disconnect();
                return false;
            }
        }

        public void Disconnect()
        {
            string userToDisconnect = connectedUsername;
            Console.WriteLine(
                $"Disconnecting Social Service. Current user: {userToDisconnect}, Proxy state: {proxy?.State}");

            if (proxy != null && proxy.State == CommunicationState.Opened && !string.IsNullOrEmpty(userToDisconnect))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await proxy.disconnectAsync(userToDisconnect);
                        Console.WriteLine($"Social Service disconnectAsync('{userToDisconnect}') called successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error calling disconnectAsync for {userToDisconnect}: {ex.Message}");
                    }
                });
            }
            else if (!string.IsNullOrEmpty(userToDisconnect))
            {
                Console.WriteLine($"Skipping server disconnect call (Proxy State: {proxy?.State}).");
            }

            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Opened || proxy.State == CommunicationState.Opening)
                    {
                        proxy.Close();
                        Console.WriteLine("Social Service WCF Channel Closed.");
                    }
                    else if (proxy.State != CommunicationState.Closed)
                    {
                        proxy.Abort();
                        Console.WriteLine($"Social Service WCF Channel Aborted from state {proxy.State}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during WCF channel close/abort: {ex.Message}. Aborting.");
                proxy?.Abort();
            }
            finally
            {
                proxy = null;
                site = null;
                callbackHandler = null;
                connectedUsername = null;
                Console.WriteLine("Social Service local resources cleaned up.");
            }
        }

        public bool EnsureConnected(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            if (proxy == null || proxy.State == CommunicationState.Closed ||
                proxy.State == CommunicationState.Faulted || connectedUsername != username)
            {
                Console.WriteLine(
                    $"EnsureConnected: Need to connect/reconnect for {username}. Current state: {proxy?.State}, Current user: {connectedUsername}");
                return Connect(username);
            }

            if (proxy.State == CommunicationState.Opening || proxy.State == CommunicationState.Created)
            {
                Console.WriteLine($"EnsureConnected: Proxy is busy ({proxy.State}) for {username}. Not ready.");
                return false;
            }

            return proxy.State == CommunicationState.Opened && connectedUsername == username;
        }
    }
}