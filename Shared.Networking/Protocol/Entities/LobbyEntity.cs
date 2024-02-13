using System;
using System.Collections.Generic;
using Shared.Networking.Common.Entities;
using Shared.Networking.Protocol.Enums;

namespace Shared.Networking.Protocol.Entities
{
    [Serializable]
    public sealed class LobbyEntity : UniqueEntity
    {
        public LobbyEntity(string name, int maxPlayerCount)
        {
            Name = name;
            MaxPlayerCount = maxPlayerCount;
        }

        public string Name { get; set; }

        public int MaxPlayerCount { get; set; }

        public LobbyState State { get; set; } = LobbyState.Waiting;

        public HashSet<AccountEntity> CurrentPlayers { get; set; } = new HashSet<AccountEntity>();

        public override string ToString() => Name;
    }
}