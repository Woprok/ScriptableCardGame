using Shared.Game.Entities.Cards.Interfaces;

namespace Shared.Game.Views
{
    public interface ICardView
    {
        ICardViewModel ContextModel { get; }
    }

    public interface ICardViewModel
    {
        void SetCard(ICard card);
        ICard ContentCard { get; }
    }
}