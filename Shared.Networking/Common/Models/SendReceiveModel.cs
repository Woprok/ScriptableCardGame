using System.Net.Sockets;
using System.Threading.Tasks;
using Shared.Networking.Common.Interfaces;
using Shared.Networking.Protocol.Entities;

namespace Shared.Networking.Common.Models
{
    public sealed class SendReceiveModel<T> : StartStopModel, ISendReceiveModel<T>
    {
        public event SendReceiveModelDisconnected<T> OnSendReceiveModelDisconnected;
        public event SendReceiveModelDataSent<T> OnSendReceiveModelDataSent;
        public event SendReceiveModelDataReceived<T> OnSendReceiveModelDataReceived;

        public SendReceiveModel(TcpClient client)
        {
            Client = client;
        }

        public TcpClient Client { get; }
        public IReceiver<T> Receiver { get; private set; }
        public ISender<T> Sender { get; private set; }
        public AccountEntity ReqisteredAccount { get; set; }
        public bool IsValidated { get { return ReqisteredAccount != null; } }

        protected override void OnModelInitialize()
        {
            Receiver = new ReceiverModel<T>(Client);
            Sender = new SenderModel<T>(Client);
        }

        protected override void OnModelStart()
        {
            Receiver.OnClientDisconnected += Receiver_OnClientDisconnected;
            Receiver.OnDataReceived += Receiver_OnDataReceived;
            Sender.OnDataSent += Sender_OnDataSent;
            Task.Factory.StartNew(() => Receiver.ReceiveAsync(CurrentCancellationToken), CurrentCancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void Sender_OnDataSent(ISender<T> sender, T data) => OnSendReceiveModelDataSent?.Invoke(this, data);

        private void Receiver_OnDataReceived(IReceiver<T> receiver, T data) => OnSendReceiveModelDataReceived?.Invoke(this, data);

        private void Receiver_OnClientDisconnected(IReceiver<T> receiver) => OnSendReceiveModelDisconnected?.Invoke(this);

        protected override void OnModelStop()
        {
            Receiver.OnClientDisconnected -= Receiver_OnClientDisconnected;
            Receiver.OnDataReceived -= Receiver_OnDataReceived;
            Sender.OnDataSent -= Sender_OnDataSent;
            Client.Close();
        }
        
        public void Send(T obj) => Task.Run(() => Sender.SendAsync(obj, CurrentCancellationToken));
    }
}