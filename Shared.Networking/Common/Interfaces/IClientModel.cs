using System.Net.Sockets;

namespace Shared.Networking.Common.Interfaces
{
    public interface IClientModel : IConnector
    {
        TcpClient Client { get; }
    }
}