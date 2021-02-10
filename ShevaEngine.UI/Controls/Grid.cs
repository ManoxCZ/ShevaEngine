using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Panel class.
	/// </summary>		
	public class Grid : Control
    {
        public List<GridRowDefinition> RowDefinitions { get; set; } = new List<GridRowDefinition>();
        public List<GridColumnDefinition> ColumnDefinitions { get; set; } = new List<GridColumnDefinition>();


        /// <summary>
        /// Constructor.
        /// </summary>
        public Grid()
        {
            RowDefinitions.Add(new GridRowDefinition { Units = Units.Relative, Height = 1 });
            ColumnDefinitions.Add(new GridColumnDefinition { Units = Units.Relative, Width = 1 });
        }

        /// <summary>
        /// Resize method.
        /// </summary>        
        public override void Resize(Rectangle locationSize)
        {
            int[] columnWidths = GetColumnWidths(locationSize.Width);
            int[] rowHeights = GetRowHeights(locationSize.Height);

            foreach (Control child in Children)
                child.Resize(new Rectangle(
					locationSize.X + GetX(columnWidths,child.GridColumn),
					locationSize.Y + GetY(rowHeights, child.GridRow), 
                    columnWidths[child.GridColumn],
					rowHeights[child.GridRow]));

            LocationSize = locationSize;
        }

        /// <summary>
        /// Get row heights.
        /// </summary>
        private int[] GetRowHeights(int parentHeight)
        {
            double absoluteSum = 0;
            double relativeSum = 0;

            foreach (GridRowDefinition rowDefinition in RowDefinitions)                
            {
                if (rowDefinition.Units == Units.Absolute)
                    absoluteSum += rowDefinition.Height;
                else
                    relativeSum += rowDefinition.Height;
            }

            double heightWithoutAbsolute = parentHeight - absoluteSum;

            return RowDefinitions.Select(item =>
            {
                if (item.Units == Units.Absolute)
                    return (int)item.Height;
                else
                    return (int)(item.Height / relativeSum * heightWithoutAbsolute);
            }).ToArray();
        }

        /// <summary>
        /// Get row heights.
        /// </summary>
        private int[] GetColumnWidths(int parentWidth)
        {
            double absoluteSum = 0;
            double relativeSum = 0;

            foreach (GridColumnDefinition columnDefinition in ColumnDefinitions)                
            {
                if (columnDefinition.Units == Units.Absolute)
                    absoluteSum += columnDefinition.Width;
                else
                    relativeSum += columnDefinition.Width;
            }

            double widthWithoutAbsolute = parentWidth - absoluteSum;

            return ColumnDefinitions.Select(item =>
            {
                if (item.Units == Units.Absolute)
                    return (int)item.Width;
                else
                    return (int)(item.Width / relativeSum * widthWithoutAbsolute);
            }).ToArray();
        }

        /// <summary>
        /// Get X.
        /// </summary>
        protected int GetX(int[] columnWidths, int index)
        {
            int width = 0;

            for (int i = 0; i < index; i++)
                width += columnWidths[i];
            
            return width;
        }

		/// <summary>
		/// Get Y.
		/// </summary>
		protected int GetY(int[] rowHeights, int index)
        {
            int height = 0;

            for (int i = 0; i < index; i++)
                height += rowHeights[i];
            
            return height;
        }

		/// <summary>
		/// Is point collide.
		/// </summary>		
		public override bool IsPointCollide(int x, int y)
		{
			if (BackColor.Value == Color.Transparent && Background.Value == null)
				return false;

			return true;
		}
	}
}
