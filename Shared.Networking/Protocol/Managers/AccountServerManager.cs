using System;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Exceptions;
using Shared.Networking.Common.Interfaces;
using Shared.Networking.Common.Managers;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Protocol.Entities;
using Shared.Networking.Protocol.Models;

namespace Shared.Networking.Protocol.Managers
{
    /// <summary>
    /// Bug could happen if multiple threads will try to access InternalColection at same time
    /// </summary>
    public sealed class AccountServerManager : ServerEntityManager<AccountEntity>
    {
        public AccountServerManager(IDatabaseModel database, Action<CoreMessage> globalSendFunction) : base(database, globalSendFunction) { }

        public override void ParseRequestMessage(ISendReceiveModel<CoreMessage> exchangerModel, EntityRequestMessage requestMessage)
        {
            Response response = Response.Denied;

            switch (requestMessage.Request)
            {
                case Request.Join:
                    EntitySimpleRequestMessage<AccountEntity> parsedMessage = requestMessage as EntitySimpleRequestMessage<AccountEntity>;
                    if (parsedMessage == null)
                        break;

                    AccountEntity databaseAccount = Database.ValidateAccount(parsedMessage.Instance);

                    if (databaseAccount != null && !InternalCollection.Contains(databaseAccount)) //ToDo improve this
                    {
                        response = Response.Accepted;
                        exchangerModel.ReqisteredAccount = databaseAccount;
                    }

                    EntitySimpleResponseMessage<AccountEntity> responseMessage = new EntitySimpleResponseMessage<AccountEntity>(requestMessage) { Instance = databaseAccount, Response = response };
                    exchangerModel.Send(responseMessage);

                    if (response == Response.Accepted)
                        EntityJoined(databaseAccount);
                    break;
                //case Request.Create:
                //    //ToDo Enable creating accounts
                //    break;
                //case Request.Delete:
                //    //ToDo Enable deleting accounts
                //    break;
                //case Request.Leave:
                //    //ToDo provide option for leaving
                //    break;
                case Request.List:
                    var message = GetCollectionMessage(requestMessage);
                    GlobalSendFunction.Invoke(message);
                    break;
                default:
                    throw new UnknownEnumValueException(nameof(requestMessage.Request));
            }
        }

        public override void ParseResponseMessage(ISendReceiveModel<CoreMessage> exchangerModel, EntityResponseMessage responseMessage)
        {
            throw new NotImplementedException();
        }
    }
}