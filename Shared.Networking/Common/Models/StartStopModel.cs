using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Exceptions;
using Shared.Networking.Common.Interfaces;

namespace Shared.Networking.Common.Models
{
    public abstract class StartStopModel : ThreadModel, IStartStopModel
    {
        protected ModelState InternalModelState;

        protected StartStopModel()
        {
            InternalModelState = ModelState.New;
        }

        protected abstract void OnModelInitialize();
        protected abstract void OnModelStart();
        protected abstract void OnModelStop();

        /// <summary>
        /// Calls onModelInitialize
        /// </summary>
        public void Initialize()
        {
            lock (CriticalAccessLock)
            {
                switch (InternalModelState)
                {
                    case ModelState.New:
                        InternalModelState = ModelState.Initialized;
                        OnModelInitialize();
                        break;
                    case ModelState.Initialized:
                    case ModelState.Started:
                    case ModelState.Stopped:
                        throw new InvalidCallException(nameof(Initialize) + InternalModelState);
                    default:
                        throw new UnknownEnumValueException(nameof(ModelState));
                }
            }
        }

        /// <summary>
        /// Calls OnModelStart and initialize new TokenSource
        /// </summary>
        public void Start()
        {
            lock (CriticalAccessLock)
            {
                switch (InternalModelState)
                {
                    case ModelState.Initialized:
                        CreateToken();
                        InternalModelState = ModelState.Started;
                        OnModelStart();
                        break;
                    case ModelState.New:
                    case ModelState.Started:
                    case ModelState.Stopped:
                        throw new InvalidCallException(nameof(Start) + InternalModelState);
                    default:
                        throw new UnknownEnumValueException(nameof(ModelState));
                }
            }
        }

        /// <summary>
        /// Calls OnModelStop and do TokenSource.Cancel
        /// </summary>
        public void Stop()
        {
            lock (CriticalAccessLock)
            {
                switch (InternalModelState)
                {
                    case ModelState.Started:
                        CancelToken();
                        InternalModelState = ModelState.Stopped;
                        OnModelStop();
                        break;
                    case ModelState.New:
                    case ModelState.Initialized:
                    case ModelState.Stopped:
                        throw new InvalidCallException(nameof(Initialize) + InternalModelState);
                    default:
                        throw new UnknownEnumValueException(nameof(ModelState));
                }
            }
        }
    }
}