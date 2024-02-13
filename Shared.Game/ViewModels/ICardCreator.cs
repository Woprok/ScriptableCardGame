using System;
using Shared.Game.Entities;
using Shared.Game.Entities.Cards.Interfaces;

namespace Shared.Game.ViewModels
{
    public interface ICardCreator
    {
        event EventHandler<ICard> NewCardCreated;
        Player ThisPlayer { get; set; }
        void NewCard();
        void TestCard();
        void ResetCard();
    }
}