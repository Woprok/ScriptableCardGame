using System;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Interfaces;
using Shared.Networking.Common.Managers;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Protocol.Entities;

namespace Shared.Networking.Protocol.Managers
{
    public sealed class AccountClientManager : ClientEntityManager<AccountEntity>
    {
        public event Action OnSuccessFulLogin; 
        public AccountClientManager(Action<CoreMessage> serverSendFunction) : base(serverSendFunction) { }

        public override void ParseRequestMessage(ISendReceiveModel<CoreMessage> exchangerModel, EntityRequestMessage requestMessage)
        {
            throw new System.NotImplementedException();
        }

        public override void ParseResponseMessage(ISendReceiveModel<CoreMessage> exchangerModel, EntityResponseMessage responseMessage)
        {
            if (responseMessage.Request == null)
            {
                if (responseMessage.Response == Response.Update)
                {
                    var message = responseMessage as EntityCollectionResponseMessage<AccountEntity>;
                    if (message == null)
                        return;
                    UpdateBuddies(message.Instance);
                }
            }
            else
            {
                switch (responseMessage.Request)
                {
                    case Request.Join:
                        var message = responseMessage as EntitySimpleResponseMessage<AccountEntity>;
                        if (message == null)
                            break;

                        if (responseMessage.Response == Response.Accepted)
                        {
                            exchangerModel.ReqisteredAccount = message.Instance;
                            OnSuccessFulLogin?.Invoke();
                        }

                        InvokeCallback(responseMessage);
                        break;
                    //ToDo create logic for List if it was requested
                    //case Request.List:
                    //    InvokeCallback(responseMessage);
                    //    break;
                    //ToDo Request Create, Delete, Leave
                }
            }
        }
    }
}