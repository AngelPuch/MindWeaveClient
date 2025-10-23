using MindWeaveClient.SocialManagerService;
using System;
using System.Diagnostics; // Para Debug.WriteLine
using System.Windows; // Para MessageBox (o idealmente usar un sistema de notificaciones mejor)

namespace MindWeaveClient.Services
{
    // Esta clase implementa la interfaz de callback definida en el servicio WCF
    public class SocialCallbackHandler : ISocialManagerCallback // El nombre puede variar según cómo se generó la referencia
    {
        // Eventos para notificar al ViewModel u otras partes de la UI
        public event Action<string> FriendRequestReceived;
        public event Action<string, bool> FriendResponseReceived;
        public event Action<string, bool> FriendStatusChanged;
        // Agrega más eventos si defines más callbacks (ej. FriendRemoved)
        public event Action<string, string> LobbyInviteReceived;

        public void notifyFriendRequest(string fromUsername)
        {
            Debug.WriteLine($"Callback: Friend request received from {fromUsername}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendRequestReceived?.Invoke(fromUsername);
                // La notificación visual ahora se maneja globalmente o en SocialViewModel
            });
        }

        public void notifyFriendResponse(string fromUsername, bool accepted)
        {
            Debug.WriteLine($"Callback: Friend response from {fromUsername}. Accepted: {accepted}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendResponseReceived?.Invoke(fromUsername, accepted);
                // La notificación visual ahora se maneja globalmente o en SocialViewModel
            });
        }

        public void notifyFriendStatusChanged(string friendUsername, bool isOnline)
        {
            Debug.WriteLine($"Callback: Friend status changed for {friendUsername}. Online: {isOnline}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendStatusChanged?.Invoke(friendUsername, isOnline);
            });
        }

        public void notifyLobbyInvite(string fromUsername, string lobbyId)
        {
            Debug.WriteLine($"Callback: Lobby invite received from {fromUsername} for lobby {lobbyId}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Dispara el evento para que App.xaml.cs (o el manejador global) lo capture
                LobbyInviteReceived?.Invoke(fromUsername, lobbyId);
                // Ya no mostramos un MessageBox aquí, se hace globalmente.
            });
        }

        // --- Implementaciones obligatorias aunque no hagan nada (si la interfaz las requiere) ---
        // Debes implementar TODOS los métodos definidos en la interfaz ISocialManagerCallback generada.
        // Si la herramienta generó nombres diferentes (ej. Begin_notifyFriendRequest, End_notifyFriendRequest),
        // debes implementar esos. Aquí asumimos nombres directos.

        // Ejemplo si se generaran métodos Begin/End para operaciones asíncronas (poco común para callbacks OneWay)
        /*
        public IAsyncResult Begin_notifyFriendRequest(string fromUsername, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void End_notifyFriendRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
        */
    }
}