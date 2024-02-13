using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Shared.Networking.Common.Exceptions;
using Shared.Networking.Common.Interfaces;

namespace Shared.Networking.Common.Models
{
    public sealed class ReceiverModel<T> : DataExchangerModel, IReceiver<T>
    {
        public event DataReceived<T> OnDataReceived;
        public event ClientDisconnected<T> OnClientDisconnected;

        public ReceiverModel(TcpClient client) : base(client) { }

        public async Task ReceiveAsync(CancellationToken token)
        {
            byte[] buffer = new byte[Client.ReceiveBufferSize];
            while (IsConnected && !token.IsCancellationRequested)
            {
                MemoryStream dataMemoryStream;
                int bytesRead;
                T deserializedObject;
                using (dataMemoryStream = new MemoryStream())
                {
                    bytesRead = await ClientStream.ReadAsync(buffer, 0, buffer.Length, token);
                    while (bytesRead > buffer.Length)
                    {
                        //ToDo verify if this is ok
                        dataMemoryStream.Write(buffer, 0, bytesRead);
                        bytesRead = await ClientStream.ReadAsync(buffer, 0, buffer.Length, token);
                    }
                    
                    dataMemoryStream.Write(buffer, 0, bytesRead);
                    dataMemoryStream.Position = 0;


                    if (bytesRead == 0)
                    {
                        if (OnClientDisconnected == null)
                            throw new EventNotSubscribedException("Disconnect method in Receiver not subscribed!");
                        await Task.Run(() => OnClientDisconnected?.Invoke(this), token);
                    }
                    else
                    {
                        deserializedObject = DeserializeReceivedData(dataMemoryStream);
                        if (OnDataReceived == null)
                            throw new EventNotSubscribedException("Receive method not subscribed!");
                        await Task.Run(() => OnDataReceived?.Invoke(this, deserializedObject), token);
                    }
                }
            }
        }

        private T DeserializeReceivedData(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            object deserialized = formatter.Deserialize(stream);
            return (T)deserialized;
        }
    }
}