using System.Collections.Generic;
using System.Linq;
using System.Net;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Interfaces;
using Shared.Networking.Common.Models;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Game.Managers;
using Shared.Networking.Game.Messages.Base;
using Shared.Networking.Protocol.Entities;
using Shared.Networking.Protocol.Managers;

namespace Shared.Networking.Protocol.Models
{
    public interface IServerMessageModel<T>
    {
        IDatabaseModel DatabaseModel { get; }
        AccountServerManager AccountServerManager { get; }
        LobbyServerManager LobbyServerManager { get; }
        GameServerManager GameServerManager { get; }
    }

    public class ServerMessageModel : GenericDataModel<CoreMessage, ListenerModel>, IServerMessageModel<CoreMessage>
    {
        public ServerMessageModel(IPEndPoint ipEndPoint, int defaultBufferSize = DefaultBufferSize) : base(ipEndPoint, defaultBufferSize)
        {
            DatabaseModel = new FakeDatabaseModel();
            AccountServerManager = new AccountServerManager(DatabaseModel, SendGlobalMessage);
            LobbyServerManager = new LobbyServerManager(DatabaseModel, SendGlobalMessage);
            GameServerManager = new GameServerManager(LobbyServerManager.TryLobbyStateChange, GetFilteredExchangers);
            Initialize();
            Start();
        }

        public IDatabaseModel DatabaseModel { get; }
        public AccountServerManager AccountServerManager { get; }
        public LobbyServerManager LobbyServerManager { get; }
        public GameServerManager GameServerManager { get; }

        public List<ISendReceiveModel<CoreMessage>> GetFilteredExchangers(HashSet<AccountEntity> selectedAccounts)
        {
            lock (SynchronizedDataExchangersAccess)
            {
                return DataExchangers.Where(item => selectedAccounts.Contains(item.ReqisteredAccount)).ToList();
            }
        }

        protected override void DataExchangerDataReceived(ISendReceiveModel<CoreMessage> exchangerModel, CoreMessage data)
        {
            if (data is RequestMessage)
                RequestParser(exchangerModel, (RequestMessage)data);
            if (data is ResponseMessage)
                ResponseParser(exchangerModel, (ResponseMessage)data);
            if (data is GameMessage message)
            {
                GameServerManager.ParseGameMessage(exchangerModel, message);
            }
        }

        private void RequestParser(ISendReceiveModel<CoreMessage> exchangerModel, RequestMessage data)
        {
            if (data.GetType() == typeof(EntitySimpleRequestMessage<AccountEntity>) || data.GetType() == typeof(EntityCollectionRequestMessage<AccountEntity>))
            {
                EntityRequestMessage parsed = (EntityRequestMessage)data;
                if (exchangerModel.IsValidated || (parsed.Request == Request.Create || parsed.Request == Request.Join))
                {
                    AccountServerManager.ParseRequestMessage(exchangerModel, parsed);
                }
            }

            //Account Operations are availble without being logged in, others require account
            if (!exchangerModel.IsValidated)
            {
                //ToDo send response
                return;
            }

            if (data.GetType() == typeof(EntitySimpleRequestMessage<LobbyEntity>) || data.GetType() == typeof(EntityCollectionRequestMessage<LobbyEntity>))
            {
                LobbyServerManager.ParseRequestMessage(exchangerModel, (EntityRequestMessage)data);
            }
        }

        private void ResponseParser(ISendReceiveModel<CoreMessage> exchangerModel, ResponseMessage data)
        {

        }
        
        protected override void DataExchangerDisconnected(ISendReceiveModel<CoreMessage> exchangerModel)
        {
            //for case of shutdown where clients did not disconnect
            //todo make this subscribeable event in managers
            LobbyServerManager.OnClientLost(exchangerModel.ReqisteredAccount);
            AccountServerManager.EntityLeft(exchangerModel.ReqisteredAccount);
            GameServerManager.OnAccountDisconnected(exchangerModel.ReqisteredAccount);

            base.DataExchangerDisconnected(exchangerModel);
        }
    }
}