using System;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Interfaces;
using Shared.Networking.Common.Managers;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Protocol.Entities;

namespace Shared.Networking.Protocol.Managers
{
    public sealed class LobbyClientManager : ClientEntityManager<LobbyEntity>
    {
        public LobbyClientManager(Action<CoreMessage> serverSendFunction) : base(serverSendFunction) { }

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
                    var message = responseMessage as EntityCollectionResponseMessage<LobbyEntity>;
                    if (message == null)
                        return;
                    UpdateBuddies(message.Instance);
                }
            }
            if (responseMessage.Request == Request.List)
            {
                if (responseMessage.Response == Response.Accepted || responseMessage.Response == Response.Update)
                {
                    var message = responseMessage as EntityCollectionResponseMessage<LobbyEntity>;
                    if (message == null)
                        return;
                    UpdateBuddies(message.Instance);
                }
            }
            else InvokeCallback(responseMessage);
        }

        public void UpdateCollection()
        {
            SendRequestToPrimaryServer(Request.List);
        }
    }
}