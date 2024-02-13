using System.Collections.Generic;
using System.Windows.Controls;
using Shared.Game.Entities;

namespace Shared.Game.Controls
{
    public class ZoneGrid : Grid
    {
        public ZoneGrid(HashSet<Player> players)
        {
            Players = players;
        }

        public HashSet<Player> Players { get; set; } = new HashSet<Player>();
    }
}