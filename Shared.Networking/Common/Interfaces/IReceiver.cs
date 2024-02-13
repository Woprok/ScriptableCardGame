using System.Threading;
using System.Threading.Tasks;

namespace Shared.Networking.Common.Interfaces
{
    public delegate void DataReceived<T>(IReceiver<T> receiver, T data);
    public delegate void ClientDisconnected<T>(IReceiver<T> receiver);

    public interface IReceiver<T>
    {
        event DataReceived<T> OnDataReceived;
        event ClientDisconnected<T> OnClientDisconnected;
        Task ReceiveAsync(CancellationToken token);
    }
}