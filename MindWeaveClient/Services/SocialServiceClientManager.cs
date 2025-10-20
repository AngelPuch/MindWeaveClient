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
                if (Proxy != null && Proxy.State != CommunicationState.Faulted)
                {
                    // Ya conectado o en proceso
                    return true;
                }

                CallbackHandler = new SocialCallbackHandler();
                site = new InstanceContext(CallbackHandler);
                // Asegúrate que el nombre del endpoint ("WSDualHttpBinding_ISocialManager" o similar)
                // coincida EXACTAMENTE con el generado en tu App.config al actualizar la referencia.
                Proxy = new SocialManagerClient(site, "WSDualHttpBinding_ISocialManager"); // <-- USA EL NOMBRE CORRECTO DE TU APP.CONFIG
                Proxy.Open(); // Abrir conexión
                return true;
            }
            catch (Exception ex)
            {
                // TODO: Log the exception
                Console.WriteLine($"Error connecting Social Service: {ex.Message}");
                Proxy = null;
                site = null;
                CallbackHandler = null;
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (Proxy != null)
                {
                    if (Proxy.State != CommunicationState.Faulted)
                    {
                        Proxy.Close();
                    }
                    Proxy = null;
                }
            }
            catch (Exception ex)
            {
                // TODO: Log the exception
                Console.WriteLine($"Error disconnecting Social Service: {ex.Message}");
                Proxy = null; // Asegurar que se limpia
            }
            finally
            {
                site = null;
                CallbackHandler = null;
            }
        }

        // Opcional: Método para reconectar si la conexión falla
        public bool EnsureConnected()
        {
            if (Proxy == null || Proxy.State == CommunicationState.Faulted || Proxy.State == CommunicationState.Closed)
            {
                Disconnect(); // Limpiar estado anterior
                return Connect();
            }
            if (Proxy.State == CommunicationState.Created || Proxy.State == CommunicationState.Opening)
            {
                // Esperar un poco o simplemente reintentar más tarde
                return false; // Indicar que no está listo aún
            }
            return true; // Ya está abierto
        }
    }
}