using System.IO;
using System.Net.Sockets;

namespace Shared.Networking.Common.Interfaces
{
    public interface IDataExchangerModel
    {
        TcpClient Client { get; }
        Stream ClientStream { get; }
        bool IsConnected { get; }
    }
}