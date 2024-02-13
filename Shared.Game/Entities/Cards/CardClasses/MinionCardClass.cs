using System;
using System.Collections.Generic;
using Shared.Common.Languages;
using Shared.Game.Entities.Cards.Interfaces;
using Shared.Game.Entities.Cards.Stats;

namespace Shared.Game.Entities.Cards.CardClasses
{
    [Serializable]
    public class MinionCardClass : ICardClass
    {
        public MinionCardClass(int power, int health)
        {
            Stats.Add(DefaultStats.Power, new PowerCardStat(power));
            Stats.Add(DefaultStats.Health, new HealthCardStat(health));
        }

        public void OnMoveTo(ICard target)
        {
            throw new NotImplementedException();
        }

        public string ClassName { get; } = LanguageHelper.TranslateContextual(nameof(MinionCardClass), "Minion");
        public Dictionary<string, ICardStat> Stats { get; set; } = new Dictionary<string, ICardStat>();
    }
}