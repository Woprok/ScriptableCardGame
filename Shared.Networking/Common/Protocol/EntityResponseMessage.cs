using System;
using Shared.Networking.Common.Enums;

namespace Shared.Networking.Common.Protocol
{
    [Serializable]
    public class EntityResponseMessage : ResponseMessage
    {
        public EntityResponseMessage(EntityRequestMessage simpleRequest) : base(simpleRequest)
        {
            Request = simpleRequest?.Request;
        }

        public Request? Request { get; set; }
        public Response Response { get; set; } = Response.Denied;
    }
}