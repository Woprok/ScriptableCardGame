using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Shared.Networking.Common.Interfaces;

namespace Shared.Networking.Common.Models
{
    public abstract class GenericDataModel<T, TConnector> : DataModel, IGenericDataModel<T, TConnector> where TConnector : class, IConnector, new()
    {
        protected GenericDataModel(IPEndPoint ipEndPoint, int defaultBufferSize = DefaultBufferSize) : base(ipEndPoint, defaultBufferSize) { }

        public TConnector Model { get; private set; }
        public List<ISendReceiveModel<T>> DataExchangers { get; set; } = new List<ISendReceiveModel<T>>();

        /// <summary>
        /// DataExchangers could be accessed from different methods at same time.
        /// </summary>
        protected readonly object SynchronizedDataExchangersAccess = new object();

        protected override void OnModelInitialize()
        {
            Model = new TConnector { IpEndPoint = IpEndPoint };
            Model.Initialize();
        }

        protected override void OnModelStart()
        {
            Model.OnNewClient += NewClientHandler;
            Model.Start();
        }

        protected override void OnModelStop()
        {
            Model.Stop();
            Model.OnNewClient -= NewClientHandler;
            lock (SynchronizedDataExchangersAccess)
            {
                DataExchangers.ForEach(exchanger =>
                {
                    exchanger.Stop();
                    exchanger.OnSendReceiveModelDataReceived -= DataExchangerDataReceived;
                    exchanger.OnSendReceiveModelDataSent -= DataExchangerDataSent;
                    exchanger.OnSendReceiveModelDisconnected -= DataExchangerDisconnected;
                });
                DataExchangers.Clear();
            }
        }


        protected void SendGlobalMessage(T message)
        {
            lock (SynchronizedDataExchangersAccess)
            {
                foreach (ISendReceiveModel<T> connectedExchanger in DataExchangers.Where(item => item.ReqisteredAccount != null && item.Client.Connected))
                {
                    connectedExchanger.Send(message);
                }
            }
        }

        /// <summary>
        /// Obtain list of all connected and validated exchangers
        /// </summary>
        protected List<ISendReceiveModel<T>> GetValidatedExchangerList()
        {
            lock (SynchronizedDataExchangersAccess)
            {
                return DataExchangers.Where(item => item.ReqisteredAccount != null && item.Client.Connected).ToList();
            }
        }

        private void NewClientHandler(TcpClient newclient)
        {
            newclient.ReceiveBufferSize = BufferSize;
            newclient.SendBufferSize = BufferSize;
            SendReceiveModel<T> exchanger = new SendReceiveModel<T>(newclient);
            exchanger.Initialize();
            exchanger.OnSendReceiveModelDataReceived += DataExchangerDataReceived;
            exchanger.OnSendReceiveModelDataSent += DataExchangerDataSent;
            exchanger.OnSendReceiveModelDisconnected += DataExchangerDisconnected;
            exchanger.Start();
            lock (SynchronizedDataExchangersAccess)
            {
                DataExchangers.Add(exchanger);
            }
        }

        protected virtual void DataExchangerDisconnected(ISendReceiveModel<T> exchangerModel)
        {
            exchangerModel.Stop();
            exchangerModel.OnSendReceiveModelDataReceived -= DataExchangerDataReceived;
            exchangerModel.OnSendReceiveModelDataSent -= DataExchangerDataSent;
            exchangerModel.OnSendReceiveModelDisconnected -= DataExchangerDisconnected;

            lock (SynchronizedDataExchangersAccess)
            {
                DataExchangers.Remove(exchangerModel);
            }
        }
        protected virtual void DataExchangerDataSent(ISendReceiveModel<T> sender, T data) { }
        protected virtual void DataExchangerDataReceived(ISendReceiveModel<T> exchangerModel, T data) { }
    }
}