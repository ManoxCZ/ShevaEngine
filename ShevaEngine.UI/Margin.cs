using Microsoft.Xna.Framework;

namespace ShevaEngine.UI
{
    public class Margin
    {
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public Margin()
        {
            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Margin(int margin)
        {
            Left = margin;
            Right = margin;
            Top = margin;
            Bottom = margin;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Margin(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public static Rectangle operator -(Rectangle locationSize, Margin margin)
        {
            return new Rectangle(locationSize.X + margin.Left, locationSize.Y + margin.Top, 
                locationSize.Width - (margin.Left + margin.Right), locationSize.Height - (margin.Top + margin.Bottom));
        }
    }
}