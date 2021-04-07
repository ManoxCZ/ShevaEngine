using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;

namespace ShevaEngine.UI
{
    /// <summary>
    /// brush
    /// </summary>
    public abstract class Brush
    {
        protected readonly Log _log;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Brush()
        {
            _log = new Log(GetType());
        }
        
        /// <summary>
        /// Draw method.
        /// </summary>
        public abstract void Draw(SpriteBatch spriteBatch, Rectangle locationSize);
    }
}
