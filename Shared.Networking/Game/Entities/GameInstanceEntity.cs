using System;
using System.Collections.Generic;
using Shared.Networking.Common.Entities;
using Shared.Networking.Game.Enums;
using Shared.Networking.Protocol.Entities;

namespace Shared.Networking.Game.Entities
{
    [Serializable]
    public class GameInstanceEntity : UniqueEntity
    {
        public GameInstanceEntity(LobbyEntity lobby)
        {
            Name = lobby.Name;
            AllPlayers = lobby.CurrentPlayers;
        }

        public string Name { get; }
        public GameState State { get; set; } = GameState.GettingReady;
        public HashSet<AccountEntity> ConnectedPlayers { get; set; } = new HashSet<AccountEntity>();
        public HashSet<AccountEntity> AllPlayers { get; set; } = new HashSet<AccountEntity>();
    }
}