using System;
using System.Collections.Generic;
using Shared.Common.CustomCollections;
using Shared.Game.Entities;
using Shared.Game.Entities.GameBoards;
using Shared.Networking.Protocol.Entities;

namespace Shared.Networking.Game.Entities
{
    [Serializable]
    public class GameStateEntity
    {
        [NonSerialized]
        protected readonly object SynchronizedAccess = new object();

        public GameStateEntity()
        {

        }

        public Dictionary<int, Player> PlayerCollection { get; set; } = new Dictionary<int, Player>();
        public int CurrentPlayer { get; set; } = 0;
        public Deque<int> Alive { get; set; } = new Deque<int>();

        public GameBoardSettings GameBoardSettings{ get; set;} = new GameBoardSettings(3,3);

        public void KillPlayer(AccountEntity account)
        {
            foreach (KeyValuePair<int, Player> player in PlayerCollection)
            {
                if (Equals(player.Value.Account, account.Id))
                {
                    if (player.Value.Avatar.IsAlive)
                    {
                        player.Value.Avatar.Kill();
                        RemovePlayerFromTurnOrder(player.Key);
                    }
                }
            }
        }

        public int MoveToNextPlayer()
        {
            lock (SynchronizedAccess)
            {
                var currentOut = Alive.PopFront();
                Alive.PushBack(currentOut);
                CurrentPlayer = Alive.PeekFront();
            }
            return CurrentPlayer;
        }

        public void RemovePlayerFromTurnOrder(int player)
        {
            lock (SynchronizedAccess)
            {
                if (Alive.Contains(player))
                    Alive.Remove(player);
            }
        }
    }
}