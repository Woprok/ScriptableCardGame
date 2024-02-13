using System.Net;

namespace Shared.Networking.Common.Interfaces
{
    public interface IDataModel
    {
        int BufferSize { get; }
        IPEndPoint IpEndPoint { get; }
    }
}