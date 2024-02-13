using System.Threading;
using System.Threading.Tasks;

namespace Shared.Networking.Common.Interfaces
{
    public delegate void DataSent<T>(ISender<T> sender, T data);

    public interface ISender<T>
    {
        event DataSent<T> OnDataSent;
        Task SendAsync(T obj, CancellationToken token);
    }
}