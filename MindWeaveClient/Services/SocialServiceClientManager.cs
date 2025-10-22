// Namespace de la referencia de servicio
using System;
using System.ServiceModel;
using MindWeaveClient.SocialManagerService; // Para InstanceContext

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

        private SocialServiceClientManager()
        {
            // Inicialización se hace en Connect
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


                CallbackHandler = new SocialCallbackHandler();
                site = new InstanceContext(CallbackHandler);

                // *** CAMBIO CLAVE AQUÍ: Usa el nombre del endpoint NetTcpBinding ***
                Proxy = new SocialManagerClient(site, "NetTcpBinding_ISocialManager"); // <-- USA EL NOMBRE CORRECTO DE TU APP.CONFIG

                Proxy.Open(); // Abrir conexión
                Console.WriteLine("Social Service Connected via NetTcpBinding."); // Mensaje de depuración
                return true;
            }
            // Captura excepciones más específicas si es necesario (TimeoutException, EndpointNotFoundException, etc.)
            catch (Exception ex)
            {
                // TODO: Log the exception more formally
                Console.WriteLine($"Error connecting Social Service: {ex.Message}");
                // Limpia todo en caso de error
                if (Proxy != null)
                {
                    if (Proxy.State != CommunicationState.Faulted)
                    {
                        try { Proxy.Abort(); } catch { /* Ignorar errores al abortar */ }
                    }
                }
                Proxy = null;
                site = null; // Site se invalida si el proxy falla
                CallbackHandler = null; // Handler se invalida si el site se invalida
                return false;
            }
        }

        public void Disconnect()
        {
            Console.WriteLine($"Disconnecting Social Service. Current state: {Proxy?.State}"); // Mensaje de depuración
            try
            {
                if (Proxy != null)
                {
                    // Solo cierra si está abierto o abriendo, Abortar si está fallido
                    if (Proxy.State == CommunicationState.Opened || Proxy.State == CommunicationState.Opening)
                    {
                        Proxy.Close();
                        Console.WriteLine("Social Service Closed."); // Mensaje de depuración
                    }
                    else if (Proxy.State == CommunicationState.Faulted)
                    {
                        Proxy.Abort();
                        Console.WriteLine("Social Service Aborted due to faulted state."); // Mensaje de depuración
                    }
                    // Si está 'Created', 'Closing', o 'Closed', no necesita acción aquí
                }
            }
            catch (CommunicationException cex) // Errores esperados al cerrar/abortar
            {
                Console.WriteLine($"CommunicationException during disconnect: {cex.Message}. Aborting.");
                Proxy?.Abort();
            }
            catch (TimeoutException tex) // Timeout al cerrar
            {
                Console.WriteLine($"TimeoutException during disconnect: {tex.Message}. Aborting.");
                Proxy?.Abort();
            }
            catch (Exception ex) // Otros errores inesperados
            {
                Console.WriteLine($"Unexpected error during disconnect: {ex.Message}. Aborting.");
                Proxy?.Abort();
            }
            finally
            {
                // Limpia referencias independientemente de si hubo error o no
                Proxy = null;
                site = null;
                CallbackHandler = null; // Asegúrate que el handler se limpie también
                Console.WriteLine("Social Service resources cleaned up."); // Mensaje de depuración
            }
        }


        // Método para reconectar si la conexión falla o se cierra
        public bool EnsureConnected()
        {
            // Si el proxy no existe, o está cerrado/fallido, intenta (re)conectar.
            if (Proxy == null || Proxy.State == CommunicationState.Closed || Proxy.State == CommunicationState.Faulted)
            {
                Console.WriteLine($"EnsureConnected: Proxy is null or in state {Proxy?.State}. Attempting to connect."); // Debug
                return Connect();
            }
            // Si está en proceso de apertura, espera (no está listo aún).
            if (Proxy.State == CommunicationState.Opening || Proxy.State == CommunicationState.Created)
            {
                Console.WriteLine($"EnsureConnected: Proxy is in state {Proxy.State}. Not ready yet."); // Debug
                // Podrías añadir un pequeño Task.Delay aquí si se llama repetidamente muy rápido
                return false;
            }
            // Si está abierto, todo bien.
            if (Proxy.State == CommunicationState.Opened)
            {
                // Console.WriteLine($"EnsureConnected: Proxy is already Opened."); // Debug (puede ser muy verboso)
                return true;
            }

            // Cualquier otro estado (Closing?) se considera no conectado de forma segura.
            Console.WriteLine($"EnsureConnected: Proxy is in unexpected state {Proxy.State}. Assuming not connected."); // Debug
            return false;

        }
    }
}