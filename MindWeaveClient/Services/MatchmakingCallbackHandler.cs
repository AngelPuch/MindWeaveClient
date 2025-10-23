// MindWeaveClient/Services/MatchmakingCallbackHandler.cs
using MindWeaveClient.MatchmakingService; // Namespace de la referencia de servicio MATCHMAKING
using System;
using System.Collections.Generic; // Para List<string>
using System.Diagnostics;
using System.Linq; // Para ToList()
using System.Windows;

namespace MindWeaveClient.Services
{
    // Implementa IMatchmakingManagerCallback (nombre puede variar)
    public class MatchmakingCallbackHandler : IMatchmakingManagerCallback
    {
        // --- Eventos ---
        // *** ELIMINADO: Ya no manejamos InviteReceived aquí ***
        // public event Action<string, string> InviteReceived;

        public event Action<LobbyStateDto> LobbyStateUpdated;
        public event Action<string, List<string>> MatchFound;
        public event Action<string> LobbyCreationFailed;
        public event Action<string> KickedFromLobby;

        // --- Implementación Callbacks ---

        // *** ELIMINADO: El método receiveLobbyInvite ya no existe en esta interfaz ***
        // public void receiveLobbyInvite(string fromUsername, string lobbyId) { ... }

        public void updateLobbyState(LobbyStateDto lobbyStateDto)
        {
            Debug.WriteLine($"Callback: Lobby state updated for lobby {lobbyStateDto?.lobbyId}");
            Application.Current.Dispatcher.Invoke(() => { LobbyStateUpdated?.Invoke(lobbyStateDto); });
        }

        // *** Implementación actualizada para usar List<string> ***
        // (Nota: La referencia de servicio actualizada DEBERÍA generar esto con List<string> si el contrato del servidor cambió)
        // Si sigue generando string[], mantén la conversión con .ToList()
        public void matchFound(string matchId, List<string> players) // Asume que la ref. actualizada usa List<string>
        {
            // List<string> playerList = players?.ToList() ?? new List<string>(); // Quita esto si ya es List<string>
            Debug.WriteLine($"Callback: Match found! ID: {matchId}. Players: {string.Join(", ", players ?? new List<string>())}"); // Usa 'players' directamente
            Application.Current.Dispatcher.Invoke(() => { MatchFound?.Invoke(matchId, players); }); // Usa 'players' directamente
        }
        // Si la referencia AÚN genera string[]:
        
        public void matchFound(string matchId, string[] players)
        {
            List<string> playerList = players?.ToList() ?? new List<string>();
            Debug.WriteLine($"Callback: Match found! ID: {matchId}. Players: {string.Join(", ", playerList)}");
            Application.Current.Dispatcher.Invoke(() => { MatchFound?.Invoke(matchId, playerList); });
        }
        


        public void lobbyCreationFailed(string reason)
        {
            Debug.WriteLine($"Callback: Lobby creation failed. Reason: {reason}");
            Application.Current.Dispatcher.Invoke(() => { LobbyCreationFailed?.Invoke(reason); });
        }

        public void kickedFromLobby(string reason)
        {
            Debug.WriteLine($"Callback: Kicked from lobby. Reason: {reason}");
            Application.Current.Dispatcher.Invoke(() => { KickedFromLobby?.Invoke(reason); });
        }

        // --- Implementaciones obligatorias adicionales ---
        // Asegúrate de implementar cualquier otro método que la interfaz IMatchmakingManagerCallback requiera
        // después de actualizar la referencia de servicio.
    }
}