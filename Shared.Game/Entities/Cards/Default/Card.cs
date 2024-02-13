using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using Shared.Common.Properties;
using Shared.Game.Entities.Cards.CardClasses;
using Shared.Game.Entities.Cards.Interfaces;

namespace Shared.Game.Entities.Cards.Default
{
    [Serializable]
    public class Card : ICard
    {
        protected static BrushConverter Convertor = new BrushConverter();

        protected Card()
        {
            SetBrush(Brushes.LightGray);
        }

        public Card(Guid currentOwner, string name, bool canBeMoved) : this()
        {
            CurrentOwner = currentOwner;
            Name = name;
            CanBeMoved = canBeMoved;
        }

        public Guid CurrentOwner { get; set; }
        public string Name { get; set; }
        public bool CanBeMoved { get; set; }

        public string RarityString { get; set; } 

        public Brush GetBrush() => (Brush)Convertor.ConvertFrom(RarityString);

        public void SetBrush(Brush brush) => RarityString = brush.ToString();

        public ICardClass Class { get; set; } = new MinionCardClass(1,1);
    }
}