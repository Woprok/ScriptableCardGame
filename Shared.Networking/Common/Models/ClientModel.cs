using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Exceptions;
using Shared.Networking.Common.Interfaces;

namespace Shared.Networking.Common.Models
{
    public sealed class ClientModel : StartStopModel, IClientModel
    {
        public event ClientObtained OnNewClient;
        
        public ClientModel() : base() { } 

        public ClientModel(IPEndPoint ipEndPoint) : this()
        {
            IpEndPoint = ipEndPoint;
        }

        public IPEndPoint IpEndPoint { get; set; }
        public TcpClient Client { get; private set; }

        protected override void OnModelInitialize()
        {
            Client = new TcpClient();
        }

        protected override void OnModelStart()
        {
            Task.Factory.StartNew(() => ConnectAsync(CurrentCancellationToken), CurrentCancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected override void OnModelStop()
        {
            Client.Close();
        }

        private async Task ConnectAsync(CancellationToken token)
        {
            await Client.ConnectAsync(IpEndPoint.Address, IpEndPoint.Port);
            if (token.IsCancellationRequested || InternalModelState == ModelState.Stopped)
                return;

            if (OnNewClient == null)
                throw new EventNotSubscribedException(nameof(ConnectAsync));
            await Task.Run(() => OnNewClient?.Invoke(Client), CurrentCancellationToken);
        }
    }
}