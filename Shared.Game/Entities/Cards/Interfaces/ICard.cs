using System;
using System.Windows.Media;

namespace Shared.Game.Entities.Cards.Interfaces
{
    public interface ICard
    {
        Guid CurrentOwner { get; set; }
        string Name { get; set; }
        bool CanBeMoved { get; set; }

        string RarityString { get; set; }
        Brush GetBrush();
        void SetBrush(Brush brush);

        ICardClass Class { get; set; }
    }
}