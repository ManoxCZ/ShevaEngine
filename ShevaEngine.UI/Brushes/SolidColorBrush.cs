using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System.Reactive.Subjects;

namespace ShevaEngine.UI.Brushes
{
    /// <summary>
    /// Solid color brush
    /// </summary>
    public class SolidColorBrush : Brush
    {
        public BehaviorSubject<Color> Color { get; }


        /// <summary>
        /// Constructor.
        /// </summary>        
        public SolidColorBrush(Color color)            
        {
            Color = new BehaviorSubject<Color>(color);
        }
        
        /// <summary>
        /// Draw method.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, Rectangle locationSize)
        {
            spriteBatch.Draw(TextureUtils.WhiteTexture, locationSize, null, Color.Value);
        }
    }
}
