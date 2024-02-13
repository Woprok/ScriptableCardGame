using System.Windows.Controls;
using Shared.Game.Entities;

namespace Shared.Game.Controls
{
    public class PlayerBoardGrid : Grid
    {
        public PlayerBoardGrid(Player player)
        {
            Player = player;
        }

        public Player Player { get; set; }
    }
}