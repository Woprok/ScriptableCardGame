using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Exceptions;
using Shared.Networking.Common.Interfaces;

namespace Shared.Networking.Common.Models
{
    public sealed class ListenerModel : StartStopModel, IListenerModel
    {
        public event ClientObtained OnNewClient;

        public ListenerModel() : base() { }

        public ListenerModel(IPEndPoint ipEndPoint) : this()
        {
            IpEndPoint = ipEndPoint;
        }

        public IPEndPoint IpEndPoint { get; set; }
        public TcpListener Listener { get; private set; }
        
        protected override void OnModelInitialize()
        {
            Listener = new TcpListener(IpEndPoint);
        }

        protected override void OnModelStart()
        {
            Listener.Start();
            Task.Factory.StartNew(() => AcceptConnectionAsync(CurrentCancellationToken), CurrentCancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected override void OnModelStop()
        {
            Listener.Stop();
        }

        private async Task AcceptConnectionAsync(CancellationToken token)
        {
            while (InternalModelState == ModelState.Started && !token.IsCancellationRequested)
            {
                TcpClient obtainedClient = await Task.Run(Listener.AcceptTcpClientAsync, token);

                if (token.IsCancellationRequested || InternalModelState == ModelState.Stopped)
                    return;

                if (OnNewClient == null)
                    throw new EventNotSubscribedException(nameof(AcceptConnectionAsync));
                await Task.Run(() => OnNewClient?.Invoke(obtainedClient), CurrentCancellationToken);
            }
        }
    }
}