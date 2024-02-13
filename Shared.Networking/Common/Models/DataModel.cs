using System.Net;
using Shared.Networking.Common.Interfaces;

namespace Shared.Networking.Common.Models
{
    public abstract class DataModel : StartStopModel, IDataModel
    {
        protected const int DefaultBufferSize = 1024 * 16;

        protected DataModel() : base() { }
        protected DataModel(IPEndPoint ipEndPoint, int defaultBufferSize = DefaultBufferSize) : this()
        {
            IpEndPoint = ipEndPoint;
            BufferSize = defaultBufferSize;
        }

        public int BufferSize { get; } = DefaultBufferSize;
        public IPEndPoint IpEndPoint { get; }
    }
}