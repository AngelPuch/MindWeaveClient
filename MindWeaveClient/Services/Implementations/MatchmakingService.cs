using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class MatchmakingService : IMatchmakingService
    {
        private MatchmakingManagerClient proxy;
        private MatchmakingCallbackHandler callbackHandler;
        private InstanceContext instanceContext;

        public event Action<LobbyStateDto> OnLobbyStateUpdated;
        public event Action<string, List<string>> OnMatchFound;
        public event Action<string> OnLobbyCreationFailed;
        public event Action<string> OnKicked;

        private void ensureClientIsCreated()
        {
            if (proxy != null && proxy.State != CommunicationState.Closed)
            {
                return;
            }
            if (proxy != null)
            {
                proxy.Abort();
            }

            callbackHandler = new MatchmakingCallbackHandler();
            instanceContext = new InstanceContext(callbackHandler);
            proxy = new MatchmakingManagerClient(instanceContext);

            callbackHandler.OnLobbyStateUpdatedEvent += handleLobbyStateUpdated;
            callbackHandler.OnMatchFoundEvent += handleMatchFound;
            callbackHandler.OnLobbyCreationFailedEvent += handleLobbyCreationFailed;
            callbackHandler.OnKickedEvent += handleKicked;
        }
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
        public async Task<LobbyCreationResultDto> createLobbyAsync(string hostUsername, LobbySettingsDto settings)
        {
            ensureClientIsCreated();
            return await executeSafeAsync(async () => await proxy.createLobbyAsync(hostUsername, settings));
        }
        public async Task<GuestJoinServiceResultDto> joinLobbyAsGuestAsync(GuestJoinRequestDto request)
        {
            ensureClientIsCreated();
            GuestJoinResultDto wcfResult = await executeSafeAsync(async () => await proxy.joinLobbyAsGuestAsync(request));

            if (wcfResult.success && wcfResult.initialLobbyState != null)
            {
                SessionService.setSession(wcfResult.assignedGuestUsername, null, true);
            }
            return new GuestJoinServiceResultDto(wcfResult, true);
        }
        public async Task joinLobbyAsync(string username, string lobbyCode)
        {
            ensureClientIsCreated();
            await executeOneWaySafeAsync(() =>
            {
                proxy.joinLobby(username, lobbyCode);
            });
        }

        public async Task leaveLobbyAsync(string username, string lobbyId)
        {
            await executeOneWaySafeAsync(() =>
            {
                proxy.leaveLobby(username, lobbyId);
            });
            disconnect();
        }
        public async Task startGameAsync(string hostUsername, string lobbyId)
        {
            await executeOneWaySafeAsync(() => proxy.startGame(hostUsername, lobbyId));
        }
        public async Task kickPlayerAsync(string hostUsername, string playerToKick, string lobbyId)
        {
            await executeOneWaySafeAsync(() => proxy.kickPlayer(hostUsername, playerToKick, lobbyId));
        }
        public async Task inviteToLobbyAsync(string inviter, string invited, string lobbyId)
        {
            await executeOneWaySafeAsync(() => proxy.inviteToLobby(inviter, invited, lobbyId));
        }
        public async Task changeDifficultyAsync(string hostUsername, string lobbyId, int newDifficultyId)
        {
            await executeOneWaySafeAsync(() => proxy.changeDifficulty(hostUsername, lobbyId, newDifficultyId));
        }
        public async Task inviteGuestByEmailAsync(GuestInvitationDto invitationData)
        {
            await executeOneWaySafeAsync(() => proxy.inviteGuestByEmail(invitationData));
        }
        public void disconnect()
        {
            if (proxy == null) return;
            try
            {
                if (proxy.State == CommunicationState.Opened) proxy.Close();
                else proxy.Abort();
            }
            catch (Exception ex)
            {
                proxy.Abort();
            }
            finally
            {
                if (callbackHandler != null)
                {
                    callbackHandler.OnLobbyStateUpdatedEvent -= handleLobbyStateUpdated;
                    callbackHandler.OnMatchFoundEvent -= handleMatchFound;
                    callbackHandler.OnLobbyCreationFailedEvent -= handleLobbyCreationFailed;
                    callbackHandler.OnKickedEvent -= handleKicked;
                }
                proxy = null;
                callbackHandler = null;
                instanceContext = null;
            }
        }

        private async Task<T> executeSafeAsync<T>(Func<Task<T>> call)
        {
            if (proxy == null || proxy.State == CommunicationState.Faulted)
            {
                throw new InvalidOperationException(Lang.ErrorMsgServerOffline);
            }
            try
            {
                return await call.Invoke();
            }
            catch (Exception)
            {
                proxy.Abort(); proxy = null;
                throw;
            }
        }

        private async Task executeOneWaySafeAsync(Action call)
        {
            if (proxy == null || proxy.State == CommunicationState.Faulted)
            {
                throw new InvalidOperationException(Lang.ErrorMsgServerOffline);
            }
            try
            {
                await Task.Run(call);
            }
            catch (Exception)
            {
                proxy.Abort(); proxy = null;
                throw;
            }
        }

        public void sendPiecePlaced(int pieceId)
        {
            try
            {
                // Esta llamada fallará hasta que hagas el Paso 3.3
                _client.sendPiecePlacedAsync(pieceId); // Usamos Async para no bloquear
            }
            catch (Exception ex)
            {
                _dialogService.showError(ex.Message, "Error");
            }
        }
    }
}