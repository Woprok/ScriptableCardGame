using System;
using Shared.Game.Entities.Cards.Interfaces;

namespace Shared.Game.Entities.Cards.Stats
{
    [Serializable]
    public class PowerCardStat : ICardStat
    {
        public PowerCardStat(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }
}