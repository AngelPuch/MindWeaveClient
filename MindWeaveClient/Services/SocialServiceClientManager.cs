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
        public static SocialServiceClientManager Instance { get { return lazy.Value; } }

        public SocialManagerClient Proxy { get; private set; }
        public SocialCallbackHandler CallbackHandler { get; private set; }
        private InstanceContext site;
        private string connectedUsername = null;

        private SocialServiceClientManager() { }

        public bool Connect(string username) 
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Social Service Connect Error: Username is required.");
                return false;
            }

            try
            {
                if (Proxy != null && Proxy.State == CommunicationState.Opened && connectedUsername == username)
                {
                    Console.WriteLine($"Social Service already connected for user {username}.");
                    return true;
                }

                if (Proxy != null)
                {
                    Console.WriteLine($"Social Service exists but state is {Proxy.State} or user mismatch. Disconnecting before reconnecting.");
                    Disconnect();
                }


                Console.WriteLine($"Attempting to connect Social Service for user {username}...");
                CallbackHandler = new SocialCallbackHandler();
                site = new InstanceContext(CallbackHandler);
                Proxy = new SocialManagerClient(site, "NetTcpBinding_ISocialManager");

                Proxy.Open(); 
                Console.WriteLine("Social Service WCF Channel Opened.");

                connectedUsername = username; 
                Task.Run(async () => {
                    try
                    {
                        await Proxy.connectAsync(username);
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
                Disconnect(); // Limpia todo en caso de error
                return false;
            }
        }

        public void Disconnect()
        {
            string userToDisconnect = connectedUsername;
            Console.WriteLine($"Disconnecting Social Service. Current user: {userToDisconnect}, Proxy state: {Proxy?.State}");

            if (Proxy != null && Proxy.State == CommunicationState.Opened && !string.IsNullOrEmpty(userToDisconnect))
            {
                Task.Run(async () => {
                    try
                    {
                        await Proxy.disconnectAsync(userToDisconnect);
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
                Console.WriteLine($"Skipping server disconnect call (Proxy State: {Proxy?.State}).");
            }

            try
            {
                if (Proxy != null)
                {
                    if (Proxy.State == CommunicationState.Opened || Proxy.State == CommunicationState.Opening)
                    {
                        Proxy.Close();
                        Console.WriteLine("Social Service WCF Channel Closed.");
                    }
                    else if (Proxy.State != CommunicationState.Closed)
                    {
                        Proxy.Abort();
                        Console.WriteLine($"Social Service WCF Channel Aborted from state {Proxy.State}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during WCF channel close/abort: {ex.Message}. Aborting.");
                Proxy?.Abort();
            }
            finally
            {
                Proxy = null;
                site = null;
                CallbackHandler = null;
                connectedUsername = null;
                Console.WriteLine("Social Service local resources cleaned up.");
            }
        }

        public bool EnsureConnected(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            if (Proxy == null || Proxy.State == CommunicationState.Closed || Proxy.State == CommunicationState.Faulted || connectedUsername != username)
            {
                Console.WriteLine($"EnsureConnected: Need to connect/reconnect for {username}. Current state: {Proxy?.State}, Current user: {connectedUsername}");
                return Connect(username);
            }

            if (Proxy.State == CommunicationState.Opening || Proxy.State == CommunicationState.Created)
            {
                Console.WriteLine($"EnsureConnected: Proxy is busy ({Proxy.State}) for {username}. Not ready.");
                return false; 
            }

            return Proxy.State == CommunicationState.Opened && connectedUsername == username;
        }

        public bool EnsureConnected()
        {
            return EnsureConnected(connectedUsername);
        }
    }
}