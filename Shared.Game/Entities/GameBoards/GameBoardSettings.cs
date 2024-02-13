using System;

namespace Shared.Game.Entities.GameBoards
{
    [Serializable]
    public class GameBoardSettings
    {
        public GameBoardSettings(int rowCount, int columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
    }
}