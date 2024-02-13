using Client.Launcher.Models;
using Shared.Common.Interfaces;
using Shared.Common.StateMachines;
using Shared.Networking.Protocol.Entities;
using Shared.Networking.Protocol.Models;

namespace Client.Launcher.Interfaces
{
    public interface ILoginWindow : IWindowActions
    {
        void ConfirmServer();
        void TryLogin();
        StateMachine<LoginState, LoginTrigger> StateMachine { get; }
        AccountEntity AccountEntity { get; }
        ClientMessageModel ClientMessageModel { get; }
    }
}