using MindWeaveClient.MatchmakingService;
using System;
using System.ServiceModel;

namespace MindWeaveClient.Services
{
    public sealed class MatchmakingServiceClientManager
    {
        private static readonly Lazy<MatchmakingServiceClientManager> lazy =
            new Lazy<MatchmakingServiceClientManager>(() => new MatchmakingServiceClientManager());

        public static MatchmakingServiceClientManager Instance { get { return lazy.Value; } }

        public MatchmakingManagerClient Proxy { get; private set; }
        public MatchmakingCallbackHandler CallbackHandler { get; private set; }

        private InstanceContext site;

        private MatchmakingServiceClientManager()
        {
        }

        public bool Connect()
        {
            try
            {
                if (Proxy != null && (Proxy.State == CommunicationState.Opened || Proxy.State == CommunicationState.Opening))
                {
                    return true;
                }

                if (Proxy != null)
                {
                    Disconnect();
                }

                CallbackHandler = new MatchmakingCallbackHandler();
                site = new InstanceContext(CallbackHandler);

                Proxy = new MatchmakingManagerClient(site, "NetTcpBinding_IMatchmakingManager");

                Proxy.Open();
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
            Console.WriteLine($"Disconnecting Matchmaking Service. Current state: {Proxy?.State}");
            try
            {
                if (Proxy != null)
                {
                    if (Proxy.State == CommunicationState.Opened || Proxy.State == CommunicationState.Opening)
                    {
                        Proxy.Close();
                        Console.WriteLine("Matchmaking Service Closed.");
                    }
                    else if (Proxy.State != CommunicationState.Closed)
                    {
                        Proxy.Abort();
                        Console.WriteLine($"Matchmaking Service Aborted from state {Proxy.State}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during Matchmaking Service disconnect: {ex.Message}. Aborting.");
                Proxy?.Abort();
            }
            finally
            {
                Proxy = null;
                site = null;
                CallbackHandler = null;
                Console.WriteLine("Matchmaking Service resources cleaned up.");
            }
        }

        public bool EnsureConnected()
        {
            if (Proxy == null || Proxy.State == CommunicationState.Closed || Proxy.State == CommunicationState.Faulted)
            {
                Console.WriteLine($"EnsureConnected (Matchmaking): Proxy is null or in state {Proxy?.State}. Attempting to connect.");
                return Connect();
            }
            if (Proxy.State == CommunicationState.Opening || Proxy.State == CommunicationState.Created)
            {
                Console.WriteLine($"EnsureConnected (Matchmaking): Proxy is in state {Proxy.State}. Not ready yet.");
                return false;
            }
            return Proxy.State == CommunicationState.Opened;
        }
    }
}