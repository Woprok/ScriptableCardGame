using System.Threading;
using Shared.Networking.Common.Exceptions;

namespace Shared.Networking.Common.Models
{
    public abstract class ThreadModel
    {
        private CancellationTokenSource tokenSource;
        protected readonly object CriticalAccessLock = new object();

        protected CancellationToken CurrentCancellationToken { get { return tokenSource?.Token ?? throw new InvalidCallException(nameof(CurrentCancellationToken)); } }

        protected void CreateToken() => tokenSource = tokenSource == null ? new CancellationTokenSource() : throw new InvalidCallException(nameof(CreateToken));

        protected void CancelToken() => tokenSource.Cancel();
    }
}