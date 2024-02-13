using System.IO;
using System.Net.Sockets;
using Shared.Networking.Common.Interfaces;

namespace Shared.Networking.Common.Models
{
    public abstract class DataExchangerModel : IDataExchangerModel
    {
        protected DataExchangerModel(TcpClient client)
        {
            Client = client;
        }

        public TcpClient Client { get; }
        public Stream ClientStream { get { return Client.GetStream(); } }
        public bool IsConnected { get { return Client.Connected; } }
    }
}