using System.Collections.Generic;

namespace Shared.Game.Entities.Cards.Interfaces
{
    public interface ICardClass
    {
        //void OnMoveEnd();
        //void OnFirstPlacement();
        //void OnMoveToBlank();
        void OnMoveTo(ICard target);

        string ClassName { get; }

        Dictionary<string, ICardStat> Stats { get; set; }
    }
}