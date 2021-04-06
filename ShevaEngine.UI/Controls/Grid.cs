using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Panel class.
	/// </summary>		
	public class Grid : Control
    {
        public BehaviorSubject<IReadOnlyCollection<RowDefinition>> RowDefinitions { get; set; }
        public BehaviorSubject<IReadOnlyCollection<ColumnDefinition>> ColumnDefinitions { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public Grid()
        {
            RowDefinitions = CreateProperty<IReadOnlyCollection<RowDefinition>>(
                nameof(RowDefinitions), new List<RowDefinition>() { new RowDefinition { Units = Units.Relative, Value = 1 } });

            ColumnDefinitions = CreateProperty<IReadOnlyCollection<ColumnDefinition>>(
                nameof(ColumnDefinitions), new List<ColumnDefinition>() { new ColumnDefinition { Units = Units.Relative, Value = 1 } });            
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
					locationSize.X + GetX(columnWidths,child.GridColumn.Value),
					locationSize.Y + GetY(rowHeights, child.GridRow.Value), 
                    columnWidths[child.GridColumn.Value],
					rowHeights[child.GridRow.Value]));

            LocationSize = locationSize;
        }

        /// <summary>
        /// Get row heights.
        /// </summary>
        private int[] GetRowHeights(int parentHeight)
        {
            double absoluteSum = 0;
            double relativeSum = 0;

            foreach (RowDefinition rowDefinition in RowDefinitions.Value)                
            {
                if (rowDefinition.Units == Units.Absolute)
                    absoluteSum += rowDefinition.Value;
                else
                    relativeSum += rowDefinition.Value;
            }

            double heightWithoutAbsolute = parentHeight - absoluteSum;

            return RowDefinitions.Value.Select(item =>
            {
                if (item.Units == Units.Absolute)
                    return (int)item.Value;
                else
                    return (int)(item.Value / relativeSum * heightWithoutAbsolute);
            }).ToArray();
        }

        /// <summary>
        /// Get row heights.
        /// </summary>
        private int[] GetColumnWidths(int parentWidth)
        {
            double absoluteSum = 0;
            double relativeSum = 0;

            foreach (ColumnDefinition columnDefinition in ColumnDefinitions.Value)                
            {
                if (columnDefinition.Units == Units.Absolute)
                    absoluteSum += columnDefinition.Value;
                else
                    relativeSum += columnDefinition.Value;
            }

            double widthWithoutAbsolute = parentWidth - absoluteSum;

            return ColumnDefinitions.Value.Select(item =>
            {
                if (item.Units == Units.Absolute)
                    return (int)item.Value;
                else
                    return (int)(item.Value / relativeSum * widthWithoutAbsolute);
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
			if (Background.Value == null)
				return false;

			return true;
		}
	}
}
