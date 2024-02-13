using System.Collections.Generic;

namespace Shared.Game.Entities.Cards.Stats
{
    public static class DefaultStats
    {
        public const string Power = "Power";
        public const string Health = "Health";

        public static List<string> DefaultStatCollection = new List<string>() { Power, Health };
    }
}