using System.Linq;
using System.Net;
using Shared.Networking.Common.Interfaces;
using Shared.Networking.Common.Models;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Game.Managers;
using Shared.Networking.Game.Messages.Base;
using Shared.Networking.Protocol.Entities;
using Shared.Networking.Protocol.Managers;

namespace Shared.Networking.Protocol.Models
{
    public interface IClientMessageModel<T>
    {
        AccountClientManager AccountClientManager { get; }
        LobbyClientManager LobbyClientManager { get; }
        GameClientManager GameClientManager { get; }
    }

    public class ClientMessageModel : GenericDataModel<CoreMessage, ClientModel>, IClientMessageModel<CoreMessage>
    {
        public ClientMessageModel(IPEndPoint ipEndPoint, int defaultBufferSize = DefaultBufferSize) : base(ipEndPoint, defaultBufferSize)
        {
            AccountClientManager = new AccountClientManager(SendRequest);
            LobbyClientManager = new LobbyClientManager(SendRequest);
            GameClientManager = new GameClientManager(SendRequest);
            AccountClientManager.OnSuccessFulLogin += LobbyClientManager.UpdateCollection;
            Initialize();
            Start();
        }

        private void SendRequest(CoreMessage message) => DataExchangers.First().Send(message);

        public AccountClientManager AccountClientManager { get; }
        public LobbyClientManager LobbyClientManager { get; }
        public GameClientManager GameClientManager { get; }

        protected override void DataExchangerDataReceived(ISendReceiveModel<CoreMessage> exchangerModel, CoreMessage data)
        {
            if (data is RequestMessage)
                RequestParser(exchangerModel, (RequestMessage)data);
            if (data is ResponseMessage)
                ResponseParser(exchangerModel, (ResponseMessage)data);
            if (data is GameMessage message)
            {
                GameClientManager.ParseGameMessage(exchangerModel, message);
            }
        }

        private void RequestParser(ISendReceiveModel<CoreMessage> exchangerModel, RequestMessage data)
        {

        }

        private void ResponseParser(ISendReceiveModel<CoreMessage> exchangerModel, ResponseMessage data)
        {
            if (data.GetType() == typeof(EntitySimpleResponseMessage<AccountEntity>) || data.GetType() == typeof(EntityCollectionResponseMessage<AccountEntity>))
            {
                AccountClientManager.ParseResponseMessage(exchangerModel, (EntityResponseMessage)data);
            }

            if (data.GetType() == typeof(EntitySimpleResponseMessage<LobbyEntity>) || data.GetType() == typeof(EntityCollectionResponseMessage<LobbyEntity>))
            {
                LobbyClientManager.ParseResponseMessage(exchangerModel, (EntityResponseMessage)data);
            }
        }
    }
}