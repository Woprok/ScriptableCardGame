using Shared.Networking.Common.Interfaces;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Protocol.Entities;

namespace Shared.Networking.Game.Entities
{
    public class ClientGameInstance
    {
        public ClientGameInstance(GameInstanceEntity instance, AccountEntity currentAccount, ISendReceiveModel<CoreMessage> currentServer)
        {
            Instance = instance;
            CurrentAccount = currentAccount;
            CurrentServer = currentServer;
        }

        public GameInstanceEntity Instance { get; set; }
        public GameStateEntity GameState { get; set; }

        public AccountEntity CurrentAccount { get; }
        public ISendReceiveModel<CoreMessage> CurrentServer { get; set; }
    }
}