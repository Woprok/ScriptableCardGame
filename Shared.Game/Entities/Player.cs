using System;
using System.Windows.Media;
using Shared.Game.Entities.Cards.CardClasses;
using Shared.Game.Entities.Cards.Default;
using Shared.Game.Entities.Cards.Interfaces;

namespace Shared.Game.Entities
{
    [Serializable]
    public class Player
    {
        public Player(Guid account, string name)
        {
            Account = account;
            Avatar = new PrimitiveAvatar(account, name, false);
        }
        public Player(Guid account, IAvatar avatar)
        {
            Account = account;
            Avatar = avatar;
        }

        public Guid Account { get; set; }
        public IAvatar Avatar { get; set; }

        public override bool Equals(object obj) => obj is Player pc && Equals(pc);

        public bool Equals(Player player) => this.Account == player.Account;

        public override int GetHashCode() => 17 ^ Account.GetHashCode();
    }

    [Serializable]
    public class PrimitiveAvatar : IAvatar
    {
        public PrimitiveAvatar(Guid accountId, string avatarName, bool moveable)
        {
            AvatarCard = new Card(accountId, avatarName, moveable)
            {
                Class = new AvatarCardClass(12, 12),
            };
            AvatarCard.SetBrush(Brushes.DarkOrange);
        }

        public ICard AvatarCard { get; set; }
        public bool IsAlive { get; private set; } = true;
        public void Kill()
        {
            IsAlive = false;
        }
    }

    public interface IAvatar
    {
        ICard AvatarCard { get; set; }
        bool IsAlive { get; }
        void Kill();
    }
}