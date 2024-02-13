using System;
using Shared.Game.Entities.Cards.Interfaces;

namespace Shared.Game.Entities.Cards.Stats
{
    [Serializable]
    public class HealthCardStat : ICardStat
    {
        public HealthCardStat(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }
}