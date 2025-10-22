// MindWeaveClient/Services/MatchmakingCallbackHandler.cs
using MindWeaveClient.MatchmakingService; // Namespace de la referencia de servicio
using System;
using System.Collections.Generic; // Para List<string>
using System.Diagnostics; // Para Debug.WriteLine
using System.Windows; // Para Dispatcher y MessageBox (o mejor, un sistema de notificaciones)

namespace MindWeaveClient.Services
{
    public class MatchmakingCallbackHandler : IMatchmakingManagerCallback
    {
        // --- Eventos para notificar a los ViewModels ---
        public event Action<string, string> InviteReceived; // fromUsername, lobbyId
        public event Action<LobbyStateDto> LobbyStateUpdated;
        public event Action<string, List<string>> MatchFound; // matchId, players
        public event Action<string> LobbyCreationFailed; // reason
        public event Action<string> KickedFromLobby; // reason

        // --- Implementación de los métodos de Callback ---

        public void receiveLobbyInvite(string fromUsername, string lobbyId)
        {
            Debug.WriteLine($"Callback: Lobby invite received from {fromUsername} for lobby {lobbyId}");
            Application.Current.Dispatcher.Invoke(() => // Ejecutar en hilo de UI
            {
                InviteReceived?.Invoke(fromUsername, lobbyId);
                // Opcional: Mostrar notificación al usuario
                MessageBox.Show($"You received a lobby invite from {fromUsername}!", "Lobby Invite", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        public void updateLobbyState(LobbyStateDto lobbyStateDto)
        {
            Debug.WriteLine($"Callback: Lobby state updated for lobby {lobbyStateDto?.lobbyId}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                LobbyStateUpdated?.Invoke(lobbyStateDto);
                // El ViewModel del Lobby debería suscribirse a este evento y actualizar su UI.
            });
        }

        public void matchFound(string matchId, List<string> players)
        {
            Debug.WriteLine($"Callback: Match found! ID: {matchId}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                MatchFound?.Invoke(matchId, players);
                // TODO: Navegar a la pantalla de juego o indicar que la partida va a empezar.
                MessageBox.Show($"Match starting! ID: {matchId}", "Match Found", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        public void lobbyCreationFailed(string reason)
        {
            Debug.WriteLine($"Callback: Lobby creation failed. Reason: {reason}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                LobbyCreationFailed?.Invoke(reason);
                MessageBox.Show($"Failed to create lobby: {reason}", "Lobby Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        }

        public void kickedFromLobby(string reason)
        {
            Debug.WriteLine($"Callback: Kicked from lobby. Reason: {reason}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                KickedFromLobby?.Invoke(reason);
                // TODO: Navegar fuera del lobby y mostrar mensaje.
                MessageBox.Show($"You were kicked from the lobby: {reason}", "Kicked", MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        }

        public void matchFound(string matchId, string[] players)
        {
            throw new NotImplementedException();
        }

        // --- Implementaciones obligatorias adicionales (si las hubiera) ---
        // Si la herramienta generó Begin/End, necesitarías implementarlos también,
        // aunque para OneWay es raro.
    }
}