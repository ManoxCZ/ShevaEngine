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

        /// <summary>
        /// Constructor.
        /// </summary>        
        public Margin(string value)
        {
            string[] parts = value.Split(',');

            switch (parts.Length)
            {
                case 1:
                    {
                        int margin = int.Parse(parts[0]);

                        Left = margin;
                        Right = margin;
                        Top = margin;
                        Bottom = margin;
                    }
                    break;
                case 2:
                    {
                        int horizontalMargin = int.Parse(parts[0]);
                        int verticalMargin = int.Parse(parts[1]);

                        Left = horizontalMargin;
                        Right = horizontalMargin;
                        Top = verticalMargin;
                        Bottom = verticalMargin;
                    }
                    break;
                case 4:
                    {
                        Left = int.Parse(parts[0]);
                        Right = int.Parse(parts[2]);
                        Top = int.Parse(parts[1]);
                        Bottom = int.Parse(parts[3]);
                    }
                    break;
            }
        }

        public static Rectangle operator -(Rectangle locationSize, Margin margin)
        {
            return new Rectangle(locationSize.X + margin.Left, locationSize.Y + margin.Top, 
                locationSize.Width - (margin.Left + margin.Right), locationSize.Height - (margin.Top + margin.Bottom));
        }
    }
}