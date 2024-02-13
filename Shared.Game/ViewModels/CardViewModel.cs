using System.Windows.Media;
using Shared.Common.Models;
using Shared.Game.Entities.Cards.Interfaces;
using Shared.Game.Entities.Cards.Stats;
using Shared.Game.Views;

namespace Shared.Game.ViewModels
{
    public class CardViewModel : ViewModelBase, ICardViewModel
    {
        private string cardName;
        private string cardHealth;
        private string cardPower;
        private Brush beautyLevel;
        private string cardClass;

        public void SetCard(ICard card)
        {
            ContentCard = card;
            CardName = card.Name;

            ICardStat cardStat;
            card.Class.Stats.TryGetValue(DefaultStats.Health, out cardStat);
            if (cardStat != null)
                CardHealth = cardStat.Value.ToString();
            card.Class.Stats.TryGetValue(DefaultStats.Power, out cardStat);
            if (cardStat != null)
                CardPower = cardStat.Value.ToString();

            BeautyLevel = card.GetBrush();
            CardClass = card.Class.ClassName;
        }

        public ICard ContentCard { get; private set; }

        public string CardName
        {
            get => cardName;
            set
            {
                if (cardName == value) return;
                cardName = value;
                OnPropertyChanged();
            }
        }
        public string CardHealth
        {
            get => cardHealth;
            set
            {
                if (cardHealth == value) return;
                cardHealth = value;
                OnPropertyChanged();
            }
        }
        public string CardPower
        {
            get => cardPower;
            set
            {
                if (cardPower == value) return;
                cardPower = value;
                OnPropertyChanged();
            }
        }
        public Brush BeautyLevel
        {
            get => beautyLevel;
            set
            {
                if (beautyLevel == value) return;
                beautyLevel = value;
                OnPropertyChanged();
            }
        }
        public string CardClass
        {
            get => cardClass;
            set
            {
                if (cardClass == value) return;
                cardClass = value;
                OnPropertyChanged();
            }
        }
    }
}