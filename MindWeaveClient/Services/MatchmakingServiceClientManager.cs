// MindWeaveClient/Services/MatchmakingServiceClientManager.cs
using MindWeaveClient.MatchmakingService; // Namespace de la referencia de servicio
using System;
using System.ServiceModel; // Para CommunicationState, InstanceContext, etc.

namespace MindWeaveClient.Services
{
    public sealed class MatchmakingServiceClientManager
    {
        // Singleton pattern
        private static readonly Lazy<MatchmakingServiceClientManager> lazy =
            new Lazy<MatchmakingServiceClientManager>(() => new MatchmakingServiceClientManager());

        public static MatchmakingServiceClientManager Instance { get { return lazy.Value; } }

        // Propiedades públicas para acceder al proxy y al handler
        public MatchmakingManagerClient Proxy { get; private set; }
        public MatchmakingCallbackHandler CallbackHandler { get; private set; }

        private InstanceContext site;

        private MatchmakingServiceClientManager()
        {
            // La inicialización se hace en Connect
        }

        public bool Connect()
        {
            try
            {
                // Si ya existe y está abierto o abriendo, no hagas nada
                if (Proxy != null && (Proxy.State == CommunicationState.Opened || Proxy.State == CommunicationState.Opening))
                {
                    return true;
                }

                // Si existe pero está fallido o cerrado, límpialo antes de reintentar
                if (Proxy != null)
                {
                    Disconnect(); // Llama a Disconnect para limpiar correctamente
                }

                // Crear el handler y el contexto
                CallbackHandler = new MatchmakingCallbackHandler();
                site = new InstanceContext(CallbackHandler);

                // Crear el proxy usando el nombre del endpoint del App.config y el InstanceContext
                // *** USA EL NOMBRE CORRECTO DE TU APP.CONFIG ***
                Proxy = new MatchmakingManagerClient(site, "NetTcpBinding_IMatchmakingManager"); // Ajusta el nombre si es diferente

                Proxy.Open(); // Abrir conexión
                Console.WriteLine("Matchmaking Service Connected via NetTcpBinding."); // Mensaje de depuración
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting Matchmaking Service: {ex.Message}"); // TODO: Log formal
                // Limpia todo en caso de error
                Disconnect(); // Usa Disconnect para limpiar
                return false;
            }
        }

        public void Disconnect()
        {
            Console.WriteLine($"Disconnecting Matchmaking Service. Current state: {Proxy?.State}"); // Mensaje de depuración
            try
            {
                if (Proxy != null)
                {
                    if (Proxy.State == CommunicationState.Opened || Proxy.State == CommunicationState.Opening)
                    {
                        Proxy.Close();
                        Console.WriteLine("Matchmaking Service Closed.");
                    }
                    else if (Proxy.State != CommunicationState.Closed) // Incluye Faulted, Closing, Created
                    {
                        Proxy.Abort();
                        Console.WriteLine($"Matchmaking Service Aborted from state {Proxy.State}.");
                    }
                }
            }
            catch (Exception ex) // Captura cualquier error durante el cierre/aborto
            {
                Console.WriteLine($"Exception during Matchmaking Service disconnect: {ex.Message}. Aborting.");
                Proxy?.Abort(); // Intenta abortar si falla el cierre
            }
            finally
            {
                // Limpia referencias independientemente de si hubo error o no
                Proxy = null;
                site = null;
                CallbackHandler = null; // Asegúrate que el handler se limpie también
                Console.WriteLine("Matchmaking Service resources cleaned up.");
            }
        }

        // Método para asegurar que la conexión está activa antes de usar el proxy
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
                return false; // Aún no está listo
            }
            // Si está abierto (Opened), todo bien.
            return Proxy.State == CommunicationState.Opened;
        }
    }
}