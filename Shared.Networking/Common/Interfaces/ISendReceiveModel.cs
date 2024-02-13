using System.Net.Sockets;
using Shared.Networking.Protocol.Entities;

namespace Shared.Networking.Common.Interfaces
{
    public delegate void SendReceiveModelDisconnected<T>(ISendReceiveModel<T> receiver);
    public delegate void SendReceiveModelDataSent<T>(ISendReceiveModel<T> sender, T data);
    public delegate void SendReceiveModelDataReceived<T>(ISendReceiveModel<T> receiver, T data);

    public interface ISendReceiveModel<T> : IStartStopModel
    {
        event SendReceiveModelDisconnected<T> OnSendReceiveModelDisconnected;
        event SendReceiveModelDataSent<T> OnSendReceiveModelDataSent;
        event SendReceiveModelDataReceived<T> OnSendReceiveModelDataReceived;
        TcpClient Client { get; }
        IReceiver<T> Receiver { get; }
        ISender<T> Sender { get; }
        void Send(T obj);
        AccountEntity ReqisteredAccount { get; set; }
        bool IsValidated { get; }
    }
}