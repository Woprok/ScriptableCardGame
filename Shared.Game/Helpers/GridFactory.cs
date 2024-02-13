using System.Windows;
using System.Windows.Controls;

namespace Shared.Game.Helpers
{
    public interface IGridFactory
    {
        void DefineNewRows(Grid grid, int rowCount);
        void DefineNewColumns(Grid grid, int columnCount);
        RowDefinition CreateRowDefinition();
        ColumnDefinition CreateColumnDefinition();
    }

    public class GridFactory : IGridFactory
    {
        /// <summary>
        /// Create new rows as defined by this.CreateRowDefinition
        /// </summary>
        public void DefineNewRows(Grid grid, int rowCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                grid.RowDefinitions.Add(CreateRowDefinition());
            }
        }

        /// <summary>
        /// Create new columns as defined by this.CreateColumnDefinition
        /// </summary>
        public void DefineNewColumns(Grid grid, int columnCount)
        {
            for (int i = 0; i < columnCount; i++)
            {
                grid.ColumnDefinitions.Add(CreateColumnDefinition());
            }
        }

        public RowDefinition CreateRowDefinition()
        {
            return new RowDefinition {Height = GridLength.Auto};
        }

        public ColumnDefinition CreateColumnDefinition()
        {
            return new ColumnDefinition {Width = GridLength.Auto};
        }
    }
}