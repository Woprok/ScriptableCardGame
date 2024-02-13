using System;
using Shared.Networking.Game.Enums;
using Shared.Networking.Game.Messages.Base;
using Shared.Networking.Protocol.Entities;

namespace Shared.Networking.Game.Messages.Requests
{
    [Serializable]
    public class CreateGameRequest : GameRequestMessage
    {
        public CreateGameRequest(LobbyEntity lobby) : base(GameRequest.CreateGame)
        {
            Lobby = lobby;
        }

        public LobbyEntity Lobby { get; set; }
    }
}