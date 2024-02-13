using System;
using System.Collections.Generic;
using Shared.Common.Languages;
using Shared.Game.Entities.Cards.Interfaces;

namespace Shared.Game.Entities.Cards.CardClasses
{
    [Serializable]
    public class ConsumableCardClass : ICardClass
    {
        public ConsumableCardClass()
        {
        }

        public void OnMoveTo(ICard target)
        {
            throw new NotImplementedException();
        }

        public string ClassName { get; } = LanguageHelper.TranslateContextual(nameof(AvatarCardClass), "Consumable");
        public Dictionary<string, ICardStat> Stats { get; set; }
    }
}