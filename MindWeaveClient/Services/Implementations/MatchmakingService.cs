using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    /// <summary>
    /// Singleton implementation of IMatchmakingService.
    /// Manages its own WCF duplex connection (proxy, context, handler)
    /// which is created when joining/creating a lobby and destroyed when leaving.
    /// </summary>
    public class MatchmakingService : IMatchmakingService
    {
        private MatchmakingManagerClient _proxy;
        private MatchmakingCallbackHandler _callbackHandler;
        private InstanceContext _instanceContext;

        // --- Eventos Públicos (de la Interfaz) ---
        public event Action<LobbyStateDto> OnLobbyStateUpdated;
        public event Action<string, List<string>> OnMatchFound;
        public event Action<string> OnLobbyCreationFailed;
        public event Action<string> OnKicked;

        private void ensureClientIsCreated()
        {
            if (_proxy != null && _proxy.State != CommunicationState.Closed)
            {
                return;
            }
            if (_proxy != null)
            {
                _proxy.Abort();
            }

            _callbackHandler = new MatchmakingCallbackHandler();
            _instanceContext = new InstanceContext(_callbackHandler);
            _proxy = new MatchmakingManagerClient(_instanceContext);

            // Suscribir el SERVICIO a los eventos del HANDLER
            _callbackHandler.OnLobbyStateUpdatedEvent += handleLobbyStateUpdated;
            _callbackHandler.OnMatchFoundEvent += handleMatchFound;
            _callbackHandler.OnLobbyCreationFailedEvent += handleLobbyCreationFailed;
            _callbackHandler.OnKickedEvent += handleKicked;
        }

        // --- Handlers Privados (El "Puente") ---
        // Estos métodos se ejecutan en el hilo de WCF y
        // simplemente disparan el evento público del servicio.

        private void handleLobbyStateUpdated(LobbyStateDto state)
        {
            OnLobbyStateUpdated?.Invoke(state);
        }

        private void handleMatchFound(string matchId, List<string> players)
        {
            OnMatchFound?.Invoke(matchId, players);
        }

        private void handleLobbyCreationFailed(string reason)
        {
            OnLobbyCreationFailed?.Invoke(reason);
        }

        private void handleKicked(string reason)
        {
            OnKicked?.Invoke(reason);
        }

        // --- Métodos de Operación ---

        public async Task<LobbyCreationResultDto> createLobbyAsync(string hostUsername, LobbySettingsDto settings)
        {
            ensureClientIsCreated();
            return await executeSafeAsync(async () =>
            {
                return await _proxy.createLobbyAsync(hostUsername, settings);
            });
        }

        public async Task<GuestJoinServiceResultDto> joinLobbyAsGuestAsync(GuestJoinRequestDto request)
        {
            ensureClientIsCreated();
            GuestJoinResultDto wcfResult = await executeSafeAsync(async () =>
            {
                return await _proxy.joinLobbyAsGuestAsync(request);
            });

            if (wcfResult.success && wcfResult.initialLobbyState != null)
            {
                SessionService.SetSession(wcfResult.assignedGuestUsername, null, true);
            }
            return new GuestJoinServiceResultDto(wcfResult, true);
        }

        public async Task joinLobbyAsync(string username, string lobbyCode)
        {
            ensureClientIsCreated();
            await executeOneWaySafeAsync(() =>
            {
                _proxy.joinLobby(username, lobbyCode);
            });
        }

        public async Task leaveLobbyAsync(string username, string lobbyId)
        {
            await executeOneWaySafeAsync(() =>
            {
                _proxy.leaveLobby(username, lobbyId);
            });
            disconnect(); // Cerrar y limpiar la conexión
        }

        public async Task startGameAsync(string hostUsername, string lobbyId)
        {
            await executeOneWaySafeAsync(() => _proxy.startGame(hostUsername, lobbyId));
        }

        public async Task kickPlayerAsync(string hostUsername, string playerToKick, string lobbyId)
        {
            // Tu contrato dice 'playerToKickUsername'
            await executeOneWaySafeAsync(() => _proxy.kickPlayer(hostUsername, playerToKick, lobbyId));
        }

        public async Task inviteToLobbyAsync(string inviter, string invited, string lobbyId)
        {
            // Tu contrato dice 'inviterUsername', 'invitedUsername'
            await executeOneWaySafeAsync(() => _proxy.inviteToLobby(inviter, invited, lobbyId));
        }

        public async Task changeDifficultyAsync(string hostUsername, string lobbyId, int newDifficultyId)
        {
            await executeOneWaySafeAsync(() => _proxy.changeDifficulty(hostUsername, lobbyId, newDifficultyId));
        }

        public async Task inviteGuestByEmailAsync(GuestInvitationDto invitationData)
        {
            await executeOneWaySafeAsync(() => _proxy.inviteGuestByEmail(invitationData));
        }

        // --- Gestión de Conexión ---

        public void disconnect()
        {
            if (_proxy == null) return;
            try
            {
                if (_proxy.State == CommunicationState.Opened) _proxy.Close();
                else _proxy.Abort();
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Unclean disconnect from MatchmakingService: {ex.Message}");
                _proxy.Abort();
            }
            finally
            {
                if (_callbackHandler != null)
                {
                    _callbackHandler.OnLobbyStateUpdatedEvent -= handleLobbyStateUpdated;
                    _callbackHandler.OnMatchFoundEvent -= handleMatchFound;
                    _callbackHandler.OnLobbyCreationFailedEvent -= handleLobbyCreationFailed;
                    _callbackHandler.OnKickedEvent -= handleKicked;
                }
                _proxy = null;
                _callbackHandler = null;
                _instanceContext = null;
            }
        }

        // --- Helpers de WCF ---

        private async Task<T> executeSafeAsync<T>(Func<Task<T>> call)
        {
            if (_proxy == null || _proxy.State == CommunicationState.Faulted)
            {
                throw new InvalidOperationException(Lang.ErrorMsgServerOffline);
            }
            try
            {
                return await call.Invoke();
            }
            catch (Exception)
            {
                _proxy.Abort(); _proxy = null;
                throw;
            }
        }

        private async Task executeOneWaySafeAsync(Action call)
        {
            if (_proxy == null || _proxy.State == CommunicationState.Faulted)
            {
                throw new InvalidOperationException(Lang.ErrorMsgServerOffline);
            }
            try
            {
                await Task.Run(call);
            }
            catch (Exception)
            {
                _proxy.Abort(); _proxy = null;
                throw;
            }
        }
    }
}