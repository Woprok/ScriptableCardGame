using System.Net;
using System.Net.Sockets;

namespace Shared.Networking.Common.Interfaces
{
    public delegate void ClientObtained(TcpClient newClient);

    public interface IConnector : IStartStopModel
    {
        event ClientObtained OnNewClient;
        IPEndPoint IpEndPoint { get; set; }
    }
}