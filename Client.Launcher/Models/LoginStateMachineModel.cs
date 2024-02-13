using System;
using Shared.Common.StateMachines;

namespace Client.Launcher.Models
{
    public enum LoginState
    {
        Initialized,
        ServerConfirmed,
        Logged
    }

    public enum LoginTrigger
    {
        ServerSelected,
        LoginFailure,
        LoginSuccess
    }

    public sealed class LoginStateMachineModel : StateMachine<LoginState, LoginTrigger>
    {
        public LoginStateMachineModel(LoginState currentState) : base(currentState) { }

        protected override void AddStates()
        {
            AddState(LoginState.Initialized, () => { }, () => { });
            AddState(LoginState.ServerConfirmed, () => { }, () => { });
            AddState(LoginState.Logged, () => { }, () => { });
        }

        protected override void AddTransitions()
        {
            AddTransition(LoginState.Initialized, LoginTrigger.ServerSelected, LoginState.ServerConfirmed, () => OnServerConfirmed.Invoke());
            AddTransition(LoginState.ServerConfirmed, LoginTrigger.LoginFailure, LoginState.ServerConfirmed, () => OnLoginFailure.Invoke());
            AddTransition(LoginState.ServerConfirmed, LoginTrigger.LoginSuccess, LoginState.Logged, () => OnLoginSuccess.Invoke());
        }

        public Action OnServerConfirmed;
        public Action OnLoginSuccess;
        public Action OnLoginFailure;
    }
}