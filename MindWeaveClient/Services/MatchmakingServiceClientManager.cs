using MindWeaveClient.MatchmakingService;
using System;
using System.ServiceModel;

namespace MindWeaveClient.Services
{
    public sealed class MatchmakingServiceClientManager
    {
        private static readonly Lazy<MatchmakingServiceClientManager> lazy =
            new Lazy<MatchmakingServiceClientManager>(() => new MatchmakingServiceClientManager());

        public static MatchmakingServiceClientManager instance { get { return lazy.Value; } }

        public MatchmakingManagerClient proxy { get; private set; }
        public MatchmakingCallbackHandler callbackHandler { get; private set; }

        private InstanceContext site;

        private MatchmakingServiceClientManager()
        {
        }

        public bool Connect()
        {
            try
            {
                if (proxy != null && (proxy.State == CommunicationState.Opened || proxy.State == CommunicationState.Opening))
                {
                    return true;
                }

                if (proxy != null)
                {
                    Disconnect();
                }

                callbackHandler = new MatchmakingCallbackHandler();
                site = new InstanceContext(callbackHandler);

                proxy = new MatchmakingManagerClient(site, "NetTcpBinding_IMatchmakingManager");

                proxy.Open();
                Console.WriteLine("Matchmaking Service Connected via NetTcpBinding.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting Matchmaking Service: {ex.Message}");
                Disconnect();
                return false;
            }
        }

        public void Disconnect()
        {
            Console.WriteLine($"Disconnecting Matchmaking Service. Current state: {proxy?.State}");
            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Opened || proxy.State == CommunicationState.Opening)
                    {
                        proxy.Close();
                        Console.WriteLine("Matchmaking Service Closed.");
                    }
                    else if (proxy.State != CommunicationState.Closed)
                    {
                        proxy.Abort();
                        Console.WriteLine($"Matchmaking Service Aborted from state {proxy.State}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during Matchmaking Service disconnect: {ex.Message}. Aborting.");
                proxy?.Abort();
            }
            finally
            {
                proxy = null;
                site = null;
                callbackHandler = null;
                Console.WriteLine("Matchmaking Service resources cleaned up.");
            }
        }

        public bool EnsureConnected()
        {
            if (proxy == null || proxy.State == CommunicationState.Closed || proxy.State == CommunicationState.Faulted)
            {
                Console.WriteLine($"EnsureConnected (Matchmaking): Proxy is null or in state {proxy?.State}. Attempting to connect.");
                return Connect();
            }
            if (proxy.State == CommunicationState.Opening || proxy.State == CommunicationState.Created)
            {
                Console.WriteLine($"EnsureConnected (Matchmaking): Proxy is in state {proxy.State}. Not ready yet.");
                return false;
            }
            return proxy.State == CommunicationState.Opened;
        }
    }
}