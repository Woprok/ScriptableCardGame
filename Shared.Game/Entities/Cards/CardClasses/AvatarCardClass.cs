using System;
using System.Collections.Generic;
using Shared.Common.Languages;
using Shared.Game.Entities.Cards.Interfaces;
using Shared.Game.Entities.Cards.Stats;

namespace Shared.Game.Entities.Cards.CardClasses
{
    [Serializable]
    public class AvatarCardClass : ICardClass
    {
        public AvatarCardClass(int power, int health)
        {
            Stats.Add(DefaultStats.Power, new PowerCardStat(power));
            Stats.Add(DefaultStats.Health, new HealthCardStat(health));
        }

        public void OnMoveTo(ICard target)
        {
            throw new NotImplementedException();
        }

        public string ClassName { get; } = LanguageHelper.TranslateContextual(nameof(AvatarCardClass), "Avatar");

        public Dictionary<string, ICardStat> Stats { get; set; } = new Dictionary<string, ICardStat>();
    }
}