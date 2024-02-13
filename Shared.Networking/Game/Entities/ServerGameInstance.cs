using System.Collections.Generic;
using System.Linq;
using Shared.Game;
using Shared.Networking.Common.Interfaces;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Protocol.Entities;

namespace Shared.Networking.Game.Entities
{
    public class ServerGameInstance
    {
        //ToDo make GameInstanceEntity threadSafe si its not required to use lock
        protected readonly object safePlayerCollection = new object();

        public ServerGameInstance(GameInstanceEntity instance, List<ISendReceiveModel<CoreMessage>> foundExchangers, LobbyEntity lobby)
        {
            Instance = instance;
            foreach (ISendReceiveModel<CoreMessage> exchanger in foundExchangers)
            {
                Exchangers.Add(exchanger.ReqisteredAccount, exchanger);
            }

            Lobby = lobby;
        }

        public VirtualServerGameBoardManager VirtualBoard { get; set; } = new VirtualServerGameBoardManager();
        public LobbyEntity Lobby { get; set; }
        public GameInstanceEntity Instance { get; set; }
        public GameStateEntity GameState { get; set; }

        public Dictionary<AccountEntity, ISendReceiveModel<CoreMessage>> Exchangers { get; set; } = new Dictionary<AccountEntity, ISendReceiveModel<CoreMessage>>();

        public void SendToAllConnected(CoreMessage message)
        {
            foreach (ISendReceiveModel<CoreMessage> sendReceiveModel in Exchangers.Values)
            {
                if (sendReceiveModel.Client.Connected)
                    sendReceiveModel.Send(message);
            }
        }

        public bool AllReady()
        {
            lock (safePlayerCollection)
            {
                return Instance.ConnectedPlayers.SetEquals(Instance.AllPlayers);
            }
        }

        public bool TryRemoveAccount(AccountEntity account)
        {
            lock (safePlayerCollection)
            {
                bool wasRemoved = false;

                //if (Exchangers.ContainsKey(account))
                //{
                //    Exchangers.Remove(account);
                //}

                if (Instance.ConnectedPlayers.Contains(account))
                {
                    Instance.ConnectedPlayers.Remove(account);
                    wasRemoved = true;
                }

                if (Instance.AllPlayers.Contains(account))
                {
                    Instance.AllPlayers.Remove(account);
                    wasRemoved = true;
                }

                if (wasRemoved && Instance.State == Enums.GameState.InProgress)
                {
                    GameState.KillPlayer(account);
                }

                return wasRemoved;
            }
        }

        public void ConnectPlayer(AccountEntity exchangerReqisteredAccount)
        {
            lock (safePlayerCollection)
            {
                Instance.ConnectedPlayers.Add(exchangerReqisteredAccount);
            }
        }
        public IList<AccountEntity> GetConnectedPlayers()
        {
            lock (safePlayerCollection)
            {
                return Instance.ConnectedPlayers.ToList();
            }
        }
    }
}