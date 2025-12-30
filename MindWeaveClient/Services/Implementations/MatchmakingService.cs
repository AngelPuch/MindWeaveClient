using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks; 
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class MatchmakingService : IMatchmakingService
    {
        private MatchmakingManagerClient proxy;
        private readonly IMatchmakingManagerCallback callbackHandler; 
       
        public event Action<LobbyStateDto> OnLobbyStateUpdated;
        public event Action<string, List<string>, LobbySettingsDto, string> OnMatchFound;
        public event Action<string> OnLobbyCreationFailed;
        public event Action<string> OnKicked;
        public event Action<string> OnLobbyActionFailed;
        public event Action<PuzzleDefinitionDto, int> OnGameStarted;
        public event Action<string> OnLobbyDestroyed;
        public event Action<string, string> OnAchievementUnlocked;

        public MatchmakingService(IMatchmakingManagerCallback callbackHandler)
        {
            this.callbackHandler = callbackHandler;
        }

        public async Task<LobbyCreationResultDto> createLobbyAsync(string hostUsername, LobbySettingsDto settings)
        {
            ensureClientIsCreated();
            return await executeServiceCallAsync(async () => await proxy.createLobbyAsync(hostUsername, settings));
        }

        public async Task<GuestJoinServiceResultDto> joinLobbyAsGuestAsync(GuestJoinRequestDto request)
        {
            ensureClientIsCreated();
            GuestJoinResultDto wcfResult = await executeServiceCallAsync(
                async () => await proxy.joinLobbyAsGuestAsync(request));

            if (wcfResult.Success && wcfResult.InitialLobbyState != null)
            {
                SessionService.setSession(wcfResult.PlayerId, wcfResult.AssignedGuestUsername, null, true);
            }
            return new GuestJoinServiceResultDto(wcfResult);
        }

        public async Task joinLobbyAsync(string username, string lobbyCode)
        {
            ensureClientIsCreated();
            await executeOneWayCallAsync(() =>
            {
                proxy.joinLobby(username, lobbyCode);
            });
        }

        public async Task leaveLobbyAsync(string username, string lobbyId)
        {
            await executeOneWayCallAsync(() =>
            {
                proxy.leaveLobby(username, lobbyId);
            });
            disconnect();
        }

        public async Task startGameAsync(string hostUsername, string lobbyId)
        {
            await executeOneWayCallAsync(() => proxy.startGame(hostUsername, lobbyId));
        }

        public async Task kickPlayerAsync(string hostUsername, string playerToKick, string lobbyId)
        {
            await executeOneWayCallAsync(() => proxy.kickPlayer(hostUsername, playerToKick, lobbyId));
        }

        public async Task inviteToLobbyAsync(string inviter, string invited, string lobbyId)
        {
            await executeOneWayCallAsync(() => proxy.inviteToLobby(inviter, invited, lobbyId));
        }

        public async Task changeDifficultyAsync(string hostUsername, string lobbyId, int newDifficultyId)
        {
            await executeOneWayCallAsync(() => proxy.changeDifficulty(hostUsername, lobbyId, newDifficultyId));
        }

        public async Task inviteGuestByEmailAsync(GuestInvitationDto invitationData)
        {
            await executeOneWayCallAsync(() => proxy.inviteGuestByEmail(invitationData));
        }
        
        public async Task requestPieceDragAsync(string lobbyCode, int pieceId)
        {
            ensureClientIsCreated(); 
            await executeTaskCallAsync(async () => await proxy.requestPieceDragAsync(lobbyCode, pieceId));

        }

        public async Task requestPieceMoveAsync(string lobbyCode, int pieceId, double newX, double newY)
        {
            ensureClientIsCreated();
            await executeTaskCallAsync(async () => await proxy.requestPieceMoveAsync(lobbyCode, pieceId, newX, newY));
        }

        public async Task requestPieceDropAsync(string lobbyCode, int pieceId, double newX, double newY)
        {
            ensureClientIsCreated();
            await executeTaskCallAsync(async () => await proxy.requestPieceDropAsync(lobbyCode, pieceId, newX, newY));
        }

        public async Task requestPieceReleaseAsync(string lobbyCode, int pieceId)
        {
            ensureClientIsCreated();
            await executeTaskCallAsync(async () => await proxy.requestPieceReleaseAsync(lobbyCode, pieceId));
        }

        public async Task leaveGameAsync(string username, string lobbyCode)
        {
            ensureClientIsCreated();
            await executeTaskCallAsync(async () => await proxy.leaveGameAsync(username, lobbyCode));
        }

        public void disconnect()
        {
            if (proxy == null) return;
            try
            {
                if (proxy.State == CommunicationState.Opened)
                {
                    proxy.Close();
                }
                else
                {
                    abortProxySafe();
                }
            }
            catch (CommunicationException)
            {
                abortProxySafe();
            }
            catch (TimeoutException)
            {
                abortProxySafe();
            }
            finally
            {
                unsubscribeFromCallbackEvents();
                proxy = null;
            }
        }

        private void ensureClientIsCreated()
        {
            if (proxy != null && proxy.State != CommunicationState.Closed)
            {
                return;
            }
            if (proxy != null)
            {
                abortProxySafe();
            }

            var instanceContext = new InstanceContext(callbackHandler);
            proxy = new MatchmakingManagerClient(instanceContext);

            subscribeToCallbackEvents();
        }

        private void subscribeToCallbackEvents()
        {
            if (!(callbackHandler is MatchmakingCallbackHandler handler))
            {
                return;
            }

            handler.OnLobbyStateUpdatedEvent += handleLobbyStateUpdated;
            handler.OnMatchFoundEvent += handleMatchFound;
            handler.OnLobbyCreationFailedEvent += handleLobbyCreationFailed;
            handler.OnLobbyActionFailedEvent += handleLobbyActionFailed;
            handler.OnKickedEvent += handleKicked;
            handler.OnLobbyDestroyedEvent += handleLobbyDestroyedCallback;
            handler.OnAchievementUnlockedEvent += handleAchievementCallback;
            handler.OnGameStartedNavigation += handleGameStartedCallback;
        }


        private void unsubscribeFromCallbackEvents()
        {
            if (callbackHandler is MatchmakingCallbackHandler handler)
            {
                handler.OnLobbyStateUpdatedEvent -= handleLobbyStateUpdated;
                handler.OnMatchFoundEvent -= handleMatchFound;
                handler.OnLobbyCreationFailedEvent -= handleLobbyCreationFailed;
                handler.OnKickedEvent -= handleKicked;
                handler.OnGameStartedNavigation -= handleGameStartedCallback;
                handler.OnLobbyDestroyedEvent -= handleLobbyDestroyedCallback;
                handler.OnAchievementUnlockedEvent -= handleAchievementCallback;
            }
        }

        private async Task<T> executeServiceCallAsync<T>(Func<Task<T>> call)
        {
            validateProxyState();

            try
            {
                return await call.Invoke();
            }
            catch (EndpointNotFoundException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (CommunicationObjectFaultedException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (CommunicationException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (TimeoutException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (SocketException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
        }

        private async Task executeOneWayCallAsync(Action call)
        {
            validateProxyState();

            try
            {
                await Task.Run(call);
            }
            catch (EndpointNotFoundException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (CommunicationObjectFaultedException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (CommunicationException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (TimeoutException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (SocketException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
        }

        private async Task executeTaskCallAsync(Func<Task> call)
        {
            validateProxyState();

            try
            {
                await call.Invoke();
            }
            catch (EndpointNotFoundException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (CommunicationObjectFaultedException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (CommunicationException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (TimeoutException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
            catch (SocketException)
            {
                abortProxySafe();
                proxy = null;
                throw;
            }
        }

        private void abortProxySafe()
        {
            try
            {
                proxy?.Abort();
            }
            catch
            {
                // Ignore
            }
        }

        private void validateProxyState()
        {
            if (proxy == null)
            {
                throw new CommunicationObjectFaultedException(Lang.ErrorMsgServerOffline);
            }

            if (proxy.State == CommunicationState.Faulted)
            {
                abortProxySafe();
                proxy = null;
                throw new CommunicationObjectFaultedException(Lang.ErrorMsgServerOffline);
            }

            if (proxy.State == CommunicationState.Closed)
            {
                proxy = null;
                throw new CommunicationObjectFaultedException(Lang.ErrorMsgServerOffline);
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

        private void handleGameStartedCallback(PuzzleDefinitionDto puzzleDefinition, int matchDurationSeconds)
        {
            OnGameStarted?.Invoke(puzzleDefinition, matchDurationSeconds);
        }

        private void handleLobbyActionFailed(string message)
        {
            OnLobbyActionFailed?.Invoke(message);
        }

        private void handleLobbyDestroyedCallback(string reason)
        {
            OnLobbyDestroyed?.Invoke(reason);
        }

        private void handleAchievementCallback(string achievementName, string imagePath)
        {
            OnAchievementUnlocked?.Invoke(achievementName, imagePath);
        }
    }
}