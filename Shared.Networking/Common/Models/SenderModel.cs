using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Shared.Networking.Common.Exceptions;
using Shared.Networking.Common.Interfaces;

namespace Shared.Networking.Common.Models
{
    public sealed class SenderModel<T> : DataExchangerModel, ISender<T>
    {
        public event DataSent<T> OnDataSent;

        public SenderModel(TcpClient client) : base(client) { }
        
        private async Task Send(byte[] data, CancellationToken token)
        {
            await ClientStream.WriteAsync(data, 0, data.Length, token);
            await ClientStream.FlushAsync(token);
        }

        public async Task SendAsync(T obj, CancellationToken token)
        {
            byte[] serializedData;
            using (MemoryStream memory = new MemoryStream())
            {
                serializedData = SerializeSendingData(obj, memory); 
            }
            if (IsConnected && !token.IsCancellationRequested)
            {
                await Task.Factory.StartNew(() => Send(serializedData, token), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                if (OnDataSent == null)
                    throw new EventNotSubscribedException("Sent method not subscribed!");
                await Task.Run(() => OnDataSent?.Invoke(this, obj), token);
            }
        }

        private byte[] SerializeSendingData(T obj, MemoryStream memory)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(memory, obj);
            return memory.ToArray();
        }
    }
}