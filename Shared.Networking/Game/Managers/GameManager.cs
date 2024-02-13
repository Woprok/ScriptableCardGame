using System;
using System.Collections.Generic;
using Shared.Networking.Common.Exceptions;
using Shared.Networking.Common.Interfaces;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Game.Enums;
using Shared.Networking.Game.Messages.Base;

namespace Shared.Networking.Game.Managers
{
    public abstract class GameManager
    {
        protected readonly object SynchronizedCurrentGames = new object();
        protected readonly object CallbackSynchronizationLock = new object();
        /// <summary>
        /// This could potentially be Delegate for more generic approach
        /// </summary>
        protected Dictionary<Guid, Action<GameResponseMessage>> CallbackDictionary { get; private set; } = new Dictionary<Guid, Action<GameResponseMessage>>();

        protected void AddCallback(GameMessage message, Action<GameResponseMessage> responseAction)
        {
            if (responseAction == null)
                return;

            message.CallbackGuid = Guid.NewGuid();

            lock (CallbackSynchronizationLock)
            {
                CallbackDictionary.Add(message.CallbackGuid.Value, responseAction);
            }
        }

        protected void InvokeCallback(GameResponseMessage responseMessage)
        {
            if (responseMessage.CallbackGuid == null)
                return;

            Action<GameResponseMessage> action = null;

            lock (CallbackSynchronizationLock)
            {
                CallbackDictionary.TryGetValue(responseMessage.CallbackGuid.Value, out action);
            }

            if (action == null)
                return;

            if (responseMessage.LastCallbackInvocation)
            {
                lock (CallbackSynchronizationLock)
                {
                    CallbackDictionary.Remove(responseMessage.CallbackGuid.Value);
                }
            }

            action.Invoke(responseMessage);
        }

        public void ParseGameMessage(ISendReceiveModel<CoreMessage> exchanger, GameMessage message)
        {
            switch (message.Request)
            {
                case GameRequest.CreateGame:
                    ProcessCreateGame(exchanger, message);
                    break;
                case GameRequest.JoinGame:
                    ProcessJoinGame(exchanger, message);
                    break;
                case GameRequest.ProcessTurn:
                    ProcessTurn(exchanger, message);
                    break;
                case GameRequest.ObtainFullState:
                    ProcessFullState(exchanger, message);
                    break;
                case GameRequest.CloseGame:
                    ProcessCloseGame(exchanger, message);
                    break;
                case null:
                    ProcessNull(exchanger, message);
                    break;
                default:
                    ProcessDefault(exchanger, message);
                    break;
            }
        }


        protected abstract void ProcessCreateGame(ISendReceiveModel<CoreMessage> exchanger, GameMessage message);
        protected abstract void ProcessJoinGame(ISendReceiveModel<CoreMessage> exchanger, GameMessage message);

        protected abstract void ProcessTurn(ISendReceiveModel<CoreMessage> exchanger, GameMessage message);
        protected abstract void ProcessFullState(ISendReceiveModel<CoreMessage> exchanger, GameMessage message);

        protected abstract void ProcessCloseGame(ISendReceiveModel<CoreMessage> exchanger, GameMessage message);

        protected abstract void ProcessNull(ISendReceiveModel<CoreMessage> exchanger, GameMessage message);
        protected void ProcessDefault(ISendReceiveModel<CoreMessage> exchanger, GameMessage message) => throw new UnknownEnumValueException(nameof(message.Request));
    }
}