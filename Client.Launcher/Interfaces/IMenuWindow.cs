using Shared.Common.Interfaces;
using Shared.Networking.Protocol.Entities;
using Shared.Networking.Protocol.Models;

namespace Client.Launcher.Interfaces
{
    public interface IMenuWindow : IWindowActions
    {
        void SetConfiguration();
        void CreateLobby();
        void JoinLobby();
        void LeaveLobby();
        void DeleteLobby();
        ClientMessageModel CurrentClientMessageModel { get; set; }
        AccountEntity CurrentAccount { get; set; }
        void StartGame();
    }
}  