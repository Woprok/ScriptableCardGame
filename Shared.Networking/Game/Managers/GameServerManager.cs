using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Game;
using Shared.Game.Entities;
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
    public sealed class GameServerManager : GameManager
    {
        private Func<HashSet<AccountEntity>, List<ISendReceiveModel<CoreMessage>>> GetGameInstanceAccounts;
        private Func<LobbyEntity, bool, bool> ValidateLobbyFunction;

        public GameServerManager(Func<LobbyEntity, bool, bool> validateLobbyFunction, Func<HashSet<AccountEntity>, List<ISendReceiveModel<CoreMessage>>> getGameInstanceAccounts)
        {
            ValidateLobbyFunction = validateLobbyFunction;
            GetGameInstanceAccounts = getGameInstanceAccounts;
        }

        /// <summary>
        /// Guid represents GameInstanceEntity ID
        /// </summary>
        public Dictionary<Guid, ServerGameInstance> CurrentGames { get; set; } = new Dictionary<Guid, ServerGameInstance>();

        public void OnAccountDisconnected(AccountEntity accountEntity)
        {
            lock (SynchronizedCurrentGames)
            {
                foreach (ServerGameInstance game in CurrentGames.Values)
                {
                    if (game.TryRemoveAccount(accountEntity))
                        Task.Run(() => game.SendToAllConnected(new FullGameUpdate(game.Instance, game.GameState)));
                }
            }
        }

        protected override void ProcessCreateGame(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            if (message is CreateGameRequest request)
            {
                bool gameInstance = false;
                if (ValidateLobbyFunction(request.Lobby, false))
                {
                    //this should mean that lobby was updated as well
                    //so it will have GameInstanceEntity with not null value
                    //sending response will cause creation of game
                    gameInstance = CreateGameInstance(request.Lobby);
                }
                //Succeded to create new game if gameinstance is not null
                //else Failed to create game
                exchanger.Send(new CreateGameResponse(request, gameInstance));
            }
        }

        private bool CreateGameInstance(LobbyEntity lobby)
        {
            List<ISendReceiveModel<CoreMessage>> foundExchangers = GetGameInstanceAccounts(lobby.CurrentPlayers);
            
            GameInstanceEntity instance = new GameInstanceEntity(lobby);
            ServerGameInstance serverInstance = new ServerGameInstance(instance, foundExchangers, lobby);
            lock (SynchronizedCurrentGames)
            {
                CurrentGames.Add(instance.Id, serverInstance);
            }

            CreateGameUpdate message = new CreateGameUpdate(instance);

            serverInstance.SendToAllConnected(message);

            return true;
        }

        protected override void ProcessJoinGame(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            if (message is JoinGameResponse response)
            {
                ServerGameInstance serverInstance;
                lock (SynchronizedCurrentGames)
                {
                    if (CurrentGames.TryGetValue(response.GameInstance, out serverInstance))
                    {
                        serverInstance.ConnectPlayer(exchanger.ReqisteredAccount);
                    }

                    if (serverInstance != null)
                    {
                        JoinGameUpdate updateMessage = new JoinGameUpdate(serverInstance.Instance);
                        serverInstance.SendToAllConnected(updateMessage);

                        if (serverInstance.AllReady() && serverInstance.Instance.State == GameState.GettingReady)
                        {
                            serverInstance.Instance.State = GameState.InProgress;
                            serverInstance.GameState = CreateStartGameState(serverInstance);
                            //todo implement spectator mode
                            //after this its possible to added spectators ? maybe
                            StartGameUpdate startMessage = new StartGameUpdate(serverInstance.Instance, serverInstance.GameState);
                            serverInstance.SendToAllConnected(startMessage);
                        }
                    }
                }
            }
        }

        private GameStateEntity CreateStartGameState(ServerGameInstance instance)
        {
            GameStateEntity state = new GameStateEntity();
            int i = 0;
            foreach (AccountEntity account in instance.GetConnectedPlayers())
            {
                state.PlayerCollection.Add(i, new Player(account.Id, account.Username));
                state.Alive.Add(i);
                i++;
            }

            instance.VirtualBoard.CreateDefaultBoard(state.PlayerCollection.Values.ToList(), state.GameBoardSettings);

            return state;
        }

        protected override void ProcessTurn(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            ServerGameInstance serverInstance = null;
            if (message is ProcessTurnRequest newTurn)
            {
                lock (SynchronizedCurrentGames)
                {
                    if (!CurrentGames.TryGetValue(newTurn.GameInstance, out serverInstance))
                        return;
                }

                TurnActionResponse actionResponse;
                if (serverInstance.GameState.PlayerCollection[serverInstance.GameState.CurrentPlayer].Account != exchanger.ReqisteredAccount.Id)
                {
                    actionResponse = TurnActionResponse.OutOfTurnOrder;
                }
                else if (!newTurn.Action.UsedAction)
                {
                    actionResponse = actionResponse = TurnActionResponse.ValidMove;
                }
                else
                {
                    var errorList = serverInstance.VirtualBoard.ProcessAction(newTurn.Action.PlayedAction, out var serverAttackResults, out var sourcePlayer, out var targetPlayer);
                    if (errorList.Count > 0)
                    {
                        actionResponse = TurnActionResponse.InvalidMove;
                    }
                    else
                    {
                        actionResponse = TurnActionResponse.ValidMove;
                        if (serverAttackResults != null && serverAttackResults.Contains(ServerAttackResult.SourceAvatarKilled))
                        {
                            serverInstance.TryRemoveAccount(serverInstance.Instance.AllPlayers.First(item => item.Id == sourcePlayer.Account));
                        }

                        if (serverAttackResults != null && serverAttackResults.Contains(ServerAttackResult.TargetAvatarKilled))
                        {
                            serverInstance.TryRemoveAccount(serverInstance.Instance.AllPlayers.First(item => item.Id == targetPlayer.Account));
                        }

                        if (serverAttackResults?.Contains(ServerAttackResult.SourceAvatarKilled) == true || serverAttackResults?.Contains(ServerAttackResult.TargetAvatarKilled) == true)
                        {
                            Task.Run(() => serverInstance.SendToAllConnected(new FullGameUpdate(serverInstance.Instance, serverInstance.GameState)));
                        }
                    }
                }

                if (actionResponse != TurnActionResponse.ValidMove)
                {
                    Task.Run(() => exchanger.Send(new ProcessTurnResponse(newTurn, actionResponse, serverInstance.GameState.CurrentPlayer)));
                }
                else
                {
                    var result = serverInstance.GameState.MoveToNextPlayer();
                    Task.Run(() => exchanger.Send(new ProcessTurnResponse(newTurn, actionResponse, result)));
                    Task.Run(() => serverInstance.SendToAllConnected(new ProcessTurnUpdate(serverInstance.Instance.Id, newTurn.Action, result)));
                }
            }

            if (serverInstance == null)
                return;

            CheckEndGameCondition(serverInstance);
        }

        private void CheckEndGameCondition(ServerGameInstance serverInstance)
        {
            if (serverInstance.Instance.State == GameState.InProgress && serverInstance.GameState.Alive.Count == 1)
            {
                serverInstance.Instance.State = GameState.Finished;
                serverInstance.SendToAllConnected(new CloseGameUpdate(serverInstance.Instance, serverInstance.GameState));

            }
        }

        private void CheckLobbyStatus(ServerGameInstance serverInstance)
        {
            if (serverInstance.Instance.State == GameState.Finished && serverInstance.Instance.ConnectedPlayers.Count == 0)
            {
                serverInstance.Lobby.State = LobbyState.Waiting;
                ValidateLobbyFunction(serverInstance.Lobby, true);
            }
        }

        protected override void ProcessFullState(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            if (message is FullGameUpdateRequest update)
            {
                lock (CurrentGames)
                {
                    if (CurrentGames.TryGetValue(update.GameInstance, out ServerGameInstance serverInstance))
                    Task.Run(() => exchanger.Send(new FullGameUpdateResponse(update, serverInstance.Instance, serverInstance.GameState)));
                }
            }
        }

        protected override void ProcessCloseGame(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            if (message is LeaveGameUpdate leaveGame)
            {
                lock (CurrentGames)
                {
                    if (CurrentGames.TryGetValue(leaveGame.Instance.Id, out ServerGameInstance serverInstance))
                    {
                        if (serverInstance.TryRemoveAccount(exchanger.ReqisteredAccount))
                            Task.Run(() => serverInstance.SendToAllConnected(new FullGameUpdate(serverInstance.Instance, serverInstance.GameState)));
                        CheckEndGameCondition(serverInstance);
                        CheckLobbyStatus(serverInstance);
                    }
                }
            }
        }
        protected override void ProcessNull(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            throw new NotImplementedException();
        }
    }
}