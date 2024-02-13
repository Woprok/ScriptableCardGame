using System;
using Shared.Common.Interfaces;
using Shared.Game.ViewModels;
using Shared.Game.Views;
using Shared.Networking.Game.Managers;

namespace Client.Launcher.GameControls.Interfaces
{
    public interface IGameWindow : IWindowActions
    {
        Guid? GameInstanceId { get; }
        GameClientManager GameManager { get; }
        void SetGameManager(Guid gameId, GameClientManager gameManager);
        void OnActivate();
        void OnUserCloseWindow();
        GameBoardView GameField { get; set; }
        void NextTurn();
    }
}