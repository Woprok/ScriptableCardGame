using System;

namespace Shared.Networking.Common.Protocol
{
    [Serializable]
    public class CoreMessage
    {
        protected static int ID = 0;

        public CoreMessage()
        {
            Id = ++ID;
        }

        public int Id { get; set; }
    }
}