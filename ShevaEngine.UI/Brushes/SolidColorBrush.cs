using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;

namespace ShevaEngine.UI.Brushes
{
    /// <summary>
    /// Solid color brush
    /// </summary>
    public class SolidColorBrush : Brush
    {
        private Color _color;
        

        /// <summary>
        /// Constructor.
        /// </summary>        
        public SolidColorBrush(Color color)            
        {
            _color = color;
        }
        
        /// <summary>
        /// Draw method.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, Rectangle locationSize)
        {
            spriteBatch.Draw(TextureUtils.WhiteTexture, locationSize, null, _color);
        }
    }
}
