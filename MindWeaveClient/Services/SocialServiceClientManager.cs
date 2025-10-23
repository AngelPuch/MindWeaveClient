// MindWeaveClient/Services/SocialServiceClientManager.cs
using System;
using System.ServiceModel;
using MindWeaveClient.SocialManagerService; // Para InstanceContext y Proxy
using System.Threading.Tasks; // Para Task

namespace MindWeaveClient.Services
{
    public sealed class SocialServiceClientManager
    {
        // Singleton (sin cambios)
        private static readonly Lazy<SocialServiceClientManager> lazy =
            new Lazy<SocialServiceClientManager>(() => new SocialServiceClientManager());
        public static SocialServiceClientManager Instance { get { return lazy.Value; } }

        public SocialManagerClient Proxy { get; private set; }
        public SocialCallbackHandler CallbackHandler { get; private set; }
        private InstanceContext site;
        private string connectedUsername = null; // Guardar el username conectado

        private SocialServiceClientManager() { }

        // *** MÉTODO Connect MODIFICADO ***
        public bool Connect(string username) // Recibe el username al conectar
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Social Service Connect Error: Username is required.");
                return false;
            }

            try
            {
                // Si ya existe y está abierto, y es el mismo usuario, no hagas nada
                if (Proxy != null && Proxy.State == CommunicationState.Opened && connectedUsername == username)
                {
                    Console.WriteLine($"Social Service already connected for user {username}.");
                    return true;
                }

                // Si existe pero está fallido, cerrado, o es para otro usuario, desconectar primero
                if (Proxy != null)
                {
                    Console.WriteLine($"Social Service exists but state is {Proxy.State} or user mismatch. Disconnecting before reconnecting.");
                    Disconnect(); // Llama a Disconnect para limpiar correctamente
                }


                Console.WriteLine($"Attempting to connect Social Service for user {username}...");
                CallbackHandler = new SocialCallbackHandler();
                site = new InstanceContext(CallbackHandler);
                // Usa el nombre del endpoint NetTcpBinding del App.config
                Proxy = new SocialManagerClient(site, "NetTcpBinding_ISocialManager");

                Proxy.Open(); // Abrir conexión física WCF
                Console.WriteLine("Social Service WCF Channel Opened.");

                connectedUsername = username; // Guardar el username

                // *** NUEVO: Llamar al método connect del SERVICIO ***
                // Es OneWay, así que no esperamos, pero lo envolvemos en Task.Run para no bloquear
                Task.Run(async () => {
                    try
                    {
                        await Proxy.connectAsync(username); // Usa el método Async generado
                        Console.WriteLine($"Social Service connectAsync('{username}') called successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error calling connectAsync for {username}: {ex.Message}");
                        // Podríamos intentar desconectar si falla el 'connect' lógico
                        // Disconnect();
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

        // *** MÉTODO Disconnect MODIFICADO ***
        public void Disconnect()
        {
            string userToDisconnect = connectedUsername; // Capturar antes de limpiar
            Console.WriteLine($"Disconnecting Social Service. Current user: {userToDisconnect}, Proxy state: {Proxy?.State}");

            // *** NUEVO: Llamar al método disconnect del SERVICIO (si hay proxy y usuario) ***
            if (Proxy != null && Proxy.State == CommunicationState.Opened && !string.IsNullOrEmpty(userToDisconnect))
            {
                // Es OneWay, no esperamos, pero lo envolvemos en Task.Run
                Task.Run(async () => {
                    try
                    {
                        await Proxy.disconnectAsync(userToDisconnect); // Usa el método Async generado
                        Console.WriteLine($"Social Service disconnectAsync('{userToDisconnect}') called successfully.");
                    }
                    catch (Exception ex)
                    {
                        // Loggear error, pero continuar con el cierre del canal local
                        Console.WriteLine($"Error calling disconnectAsync for {userToDisconnect}: {ex.Message}");
                    }
                });
            }
            else if (!string.IsNullOrEmpty(userToDisconnect))
            {
                Console.WriteLine($"Skipping server disconnect call (Proxy State: {Proxy?.State}).");
            }


            // Lógica existente para cerrar/abortar el canal WCF local
            try
            {
                if (Proxy != null)
                {
                    if (Proxy.State == CommunicationState.Opened || Proxy.State == CommunicationState.Opening)
                    {
                        Proxy.Close();
                        Console.WriteLine("Social Service WCF Channel Closed.");
                    }
                    else if (Proxy.State != CommunicationState.Closed) // Faulted, Closing, Created
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
                connectedUsername = null; // Limpiar el usuario
                Console.WriteLine("Social Service local resources cleaned up.");
            }
        }


        // EnsureConnected: Ahora debe verificar también el usuario
        public bool EnsureConnected(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            // Si no hay proxy, está cerrado/fallido, O el usuario conectado no coincide
            if (Proxy == null || Proxy.State == CommunicationState.Closed || Proxy.State == CommunicationState.Faulted || connectedUsername != username)
            {
                Console.WriteLine($"EnsureConnected: Need to connect/reconnect for {username}. Current state: {Proxy?.State}, Current user: {connectedUsername}");
                return Connect(username); // Intenta conectar/reconectar con el usuario correcto
            }

            if (Proxy.State == CommunicationState.Opening || Proxy.State == CommunicationState.Created)
            {
                Console.WriteLine($"EnsureConnected: Proxy is busy ({Proxy.State}) for {username}. Not ready.");
                return false; // Aún no está listo
            }

            // Si está abierto y el usuario coincide, todo bien.
            return Proxy.State == CommunicationState.Opened && connectedUsername == username;
        }

        // Sobrecarga para compatibilidad (usará el usuario actual si existe)
        public bool EnsureConnected()
        {
            return EnsureConnected(connectedUsername);
        }
    }
}