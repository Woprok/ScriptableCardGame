using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Game.Entities.Turns;
using Shared.Networking.Common.Interfaces;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Game.Entities;
using Shared.Networking.Game.Enums;
using Shared.Networking.Game.Messages.Base;
using Shared.Networking.Game.Messages.Requests;
using Shared.Networking.Game.Messages.Responses;
using Shared.Networking.Game.Messages.Updates;
using Shared.Networking.Protocol.Entities;
using Shared.Networking.Protocol.Enums;

namespace Shared.Networking.Game.Managers
{
    public sealed class GameClientManager : GameManager
    {
        private Action<CoreMessage> ServerSendFunction;

        /// <summary>
        /// should create window
        /// </summary>
        public event EventHandler<CreateGameUpdate> OnGameCreated; 
        /// <summary>
        /// should update connected players
        /// </summary>
        public event EventHandler<GameInstanceEntity> OnGamePlayersUpdated; 
        /// <summary>
        /// should update game state
        /// </summary>
        public event EventHandler<ClientGameInstance> OnGameStateStart; 
        public event EventHandler<ClientGameInstance> OnGameStateUpdated; 
        /// <summary>
        /// should be final state of game
        /// </summary>
        public event EventHandler<ClientGameInstance> OnGameFinished;         
        
        /// <summary>
        /// on new turn being accepted by server
        /// </summary>
        public event EventHandler<ProcessTurnUpdate> OnProcessTurn;

        public GameClientManager(Action<CoreMessage> serverSendFunction)
        {
            ServerSendFunction = serverSendFunction;
        }
        
        /// <summary>
        /// Guid represents GameInstanceEntity ID
        /// </summary>
        public Dictionary<Guid,ClientGameInstance> CurrentGames { get; set; } = new Dictionary<Guid, ClientGameInstance>();

        protected override void ProcessCreateGame(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            if (message is CreateGameResponse response)
            {
                InvokeCallback(response);
            }

            if (message is CreateGameUpdate update)
            {
                lock (SynchronizedCurrentGames)
                {
                    CurrentGames.Add(update.GameInstance.Id, new ClientGameInstance(update.GameInstance, exchanger.ReqisteredAccount, exchanger));
                }
                OnGameCreated?.Invoke(this, update);
            }
        }
        protected override void ProcessJoinGame(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            if (message is JoinGameUpdate update)
            {
                UpdateClientState(update.Instance, out ClientGameInstance clientInstance);

                if (clientInstance != null)
                    OnGamePlayersUpdated?.Invoke(this, update.Instance);
            }
        }

        private void UpdateClientState(GameInstanceEntity instance, out ClientGameInstance clientInstance)
        {
            lock (SynchronizedCurrentGames)
            {
                if (CurrentGames.TryGetValue(instance.Id, out clientInstance))
                {
                    clientInstance.Instance = instance;
                }
            }
        }

        private void UpdateClientGameState(GameInstanceEntity instance, GameStateEntity gameState, out ClientGameInstance clientInstance)
        {
            lock (SynchronizedCurrentGames)
            {
                if (CurrentGames.TryGetValue(instance.Id, out clientInstance))
                {
                    clientInstance.Instance = instance;
                    clientInstance.GameState = gameState;
                }
            }
        }

        protected override void ProcessTurn(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            if (message is ProcessTurnResponse response)
            {
                InvokeCallback(response);
            }

            if (message is ProcessTurnUpdate update)
            {
                OnProcessTurn?.Invoke(this, update);
            }
        }
        protected override void ProcessFullState(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            if (message is FullGameUpdate fullGame)
            {
                UpdateClientGameState(fullGame.Instance, fullGame.State, out ClientGameInstance clientInstance);

                if (clientInstance != null)
                    OnGameStateUpdated?.Invoke(this, clientInstance);
            }

            if (message is StartGameUpdate startGame)
            {
                UpdateClientGameState(startGame.Instance, startGame.State, out ClientGameInstance clientInstance);

                if (clientInstance != null)
                    OnGameStateStart?.Invoke(this, clientInstance);
            }
        }

        protected override void ProcessCloseGame(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            if (message is CloseGameUpdate closeGame)
            {
                UpdateClientGameState(closeGame.Instance, closeGame.State, out ClientGameInstance clientInstance);

                if (clientInstance != null)
                    OnGameFinished?.Invoke(this, clientInstance);
            }
        }

        protected override void ProcessNull(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            throw new NotImplementedException();
        }

        public void SendCreateGameRequest(LobbyEntity lobby, Action<GameResponseMessage> responseAction)
        {
            lobby.State = LobbyState.InProgress;
            CreateGameRequest message = new CreateGameRequest(lobby);
            AddCallback(message, responseAction);
            ServerSendFunction?.Invoke(message);
        }

        public void SendJoinGameResponse(Guid gameId)
        {
            lock (SynchronizedCurrentGames)
            {
                if (CurrentGames.TryGetValue(gameId, out ClientGameInstance result))
                {
                    JoinGameResponse message = new JoinGameResponse(new GameRequestMessage(GameRequest.JoinGame), gameId);
                    Task.Run(() =>result.CurrentServer.Send(message));
                }
            }
        }

        public void SendFullGameRequest(Guid instanceId, Action<GameResponseMessage> responseAction)
        {
            lock (SynchronizedCurrentGames)
            {
                if (CurrentGames.TryGetValue(instanceId, out ClientGameInstance result))
                {
                    FullGameUpdateRequest message = new FullGameUpdateRequest(result.Instance.Id);
                    AddCallback(message, responseAction);
                    Task.Run(() => result.CurrentServer.Send(message));
                }
            }
        }

        public void SendProcessTurnRequest(Guid instanceId, TurnAction action, Action<GameResponseMessage> responseAction)
        {
            lock (SynchronizedCurrentGames)
            {
                if (CurrentGames.TryGetValue(instanceId, out ClientGameInstance result))
                {
                    ProcessTurnRequest message = new ProcessTurnRequest(result.Instance.Id, action);
                    AddCallback(message, responseAction);
                    Task.Run(() => result.CurrentServer.Send(message));
                }
            }
        }

        public void SendLeaveGameUpdate(Guid instanceId)
        {
            lock (SynchronizedCurrentGames)
            {
                if (CurrentGames.TryGetValue(instanceId, out ClientGameInstance result))
                {
                    LeaveGameUpdate message = new LeaveGameUpdate(result.Instance);
                    Task.Run(() => result.CurrentServer.Send(message));
                }
            }
        }

        public ClientGameInstance GetCurrentGameInstance(Guid instanceId)
        {
            lock (SynchronizedCurrentGames)
            {
                return CurrentGames[instanceId];
            }
        }
    }
}