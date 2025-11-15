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
        private IMatchmakingManagerCallback callbackHandler; 
        private InstanceContext instanceContext;

       
        public event Action<LobbyStateDto> OnLobbyStateUpdated;
        public event Action<string, List<string>, LobbySettingsDto, string> OnMatchFound;
        public event Action<string> OnLobbyCreationFailed;
        public event Action<string> OnKicked;

        public MatchmakingService(IMatchmakingManagerCallback callbackHandler)
        {
            this.callbackHandler = callbackHandler;
        }

       
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

            
            instanceContext = new InstanceContext(callbackHandler);
            proxy = new MatchmakingManagerClient(instanceContext);

           
            if (callbackHandler is MatchmakingCallbackHandler handler)
            {
                handler.OnLobbyStateUpdatedEvent += handleLobbyStateUpdated;
                handler.OnMatchFoundEvent += handleMatchFound; 
                handler.OnLobbyCreationFailedEvent += handleLobbyCreationFailed;
                handler.OnKickedEvent += handleKicked;
            }
        }

        private void handleLobbyStateUpdated(LobbyStateDto state)
        {
            OnLobbyStateUpdated?.Invoke(state);
        }

        private void handleMatchFound(string lobbyCode, List<string> players, LobbySettingsDto settings, string puzzleImagePath)
        {
            OnMatchFound?.Invoke(lobbyCode, players, settings, puzzleImagePath);
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
            catch (Exception)
            {
                proxy.Abort();
            }
            finally
            {
                if (callbackHandler != null && callbackHandler is MatchmakingCallbackHandler handler)
                {
                    handler.OnLobbyStateUpdatedEvent -= handleLobbyStateUpdated;
                    handler.OnMatchFoundEvent -= handleMatchFound;
                    handler.OnLobbyCreationFailedEvent -= handleLobbyCreationFailed;
                    handler.OnKickedEvent -= handleKicked;
                }
                proxy = null;
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

        
        
        public async Task sendPiecePlacedAsync(int pieceId)
        {
            ensureClientIsCreated();
            await executeSafeTaskAsync(async () => await proxy.sendPiecePlacedAsync(pieceId));
        }
        

        private async Task executeSafeTaskAsync(Func<Task> call)
        {
            if (proxy == null || proxy.State == CommunicationState.Faulted)
            {
                throw new InvalidOperationException(Lang.ErrorMsgServerOffline);
            }
            try
            {
                await call.Invoke();
            }
            catch (Exception)
            {
                proxy.Abort(); proxy = null;
                throw;
            }
        }
    }
}