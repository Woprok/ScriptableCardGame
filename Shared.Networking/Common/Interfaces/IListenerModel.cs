using System.Net.Sockets;

namespace Shared.Networking.Common.Interfaces
{
    /// <summary>
    /// Use as follow:
    /// Call Initialize -> Subscribe OnClientCreated -> Call Start
    /// If needed it should be possible to UnSubscribe OnClientCreated -> Call Stop
    /// </summary>
    public interface IListenerModel : IConnector
    {
        TcpListener Listener { get; }
    }
}