using System;
using System.Collections.Generic;

namespace Shared.Common.StateMachines
{
    /// <summary>
    /// It's just cool to use this.
    /// </summary>
    public abstract class StateMachine<TState, TTrigger>
    {
        private readonly Dictionary<TState, Tuple<Action, Action>> stateDictionary = new Dictionary<TState, Tuple<Action, Action>>();
        private readonly Dictionary<Tuple<TState, TTrigger>, Tuple<TState, Action>> transitionDictionary = new Dictionary<Tuple<TState, TTrigger>, Tuple<TState, Action>>();
        private TState currentState;

        public event Action StateUpdate;

        protected StateMachine(TState state)
        {
            AddStates();
            AddTransitions();
            CurrentState = state;
        }

        protected abstract void AddStates();
        protected abstract void AddTransitions();

        protected void AddState(TState state, Action entryAction, Action exitAction)
        {
            stateDictionary[state] = new Tuple<Action, Action>(entryAction, exitAction);
        }

        protected void AddTransition(TState state, TTrigger trigger, TState nextState, Action transitionAction)
        {
            transitionDictionary[new Tuple<TState, TTrigger>(state, trigger)] = new Tuple<TState, Action>(nextState, transitionAction);
            if (!stateDictionary.ContainsKey(state))
                throw new NotSupportedException(string.Format("Adding transition from unknown state {0} not supported", state));
            if (!stateDictionary.ContainsKey(nextState))
                throw new NotSupportedException(string.Format("Adding transition to unknown state {0} not supported", nextState));
        }

        public void ProcessTrigger(TTrigger trigger)
        {
            TState previousState = CurrentState;
            Tuple<TState, TTrigger> transitionKey = new Tuple<TState, TTrigger>(previousState, trigger);
            if (!transitionDictionary.ContainsKey(transitionKey))
                throw new NotSupportedException(string.Format("Transition on trigger \"{0}\" from state \"{1}\" not defined.", trigger, previousState));

            Tuple<TState, Action> transition = transitionDictionary[transitionKey];
            TState nextState = transition.Item1;
            if (nextState.Equals(CurrentState))
                return;
            Action transitionAction = transition.Item2;
            Action previousStateExitAction = stateDictionary[previousState].Item2;
            Action nextStateEntryAction = stateDictionary[nextState].Item1;

            previousStateExitAction();
            transitionAction();
            nextStateEntryAction();
            CurrentState = nextState;
            OnStateUpdate();
        }

        public TState CurrentState
        {
            get { return currentState; }
            set
            {
                if (Equals(currentState, value)) return;
                currentState = value;
                stateDictionary[value].Item1();
            }
        }

        private void OnStateUpdate() => StateUpdate?.Invoke();
    }
}