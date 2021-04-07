using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System;
using System.Linq;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Effect brush.
    /// </summary>
    public class EffectBrush : Brush
    {
        protected Effect _effect;
        public BehaviorSubject<Color> ColorTint { get; }
        public Texture2D Texture;


        /// <summary>
        /// Constructor.
        /// </summary>
        public EffectBrush(string effectFilename)
        {
            ColorTint = new BehaviorSubject<Color>(Color.White);
            _effect = ShevaGame.Instance.Content.Load<Effect>(effectFilename);
            Texture = TextureUtils.WhiteTexture;
        }

        /// <summary>
        /// Draw.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, Rectangle locationSize)
        {
            if (_effect == null)
                return;

            spriteBatch.End();

            Viewport vp = spriteBatch.GraphicsDevice.Viewport;

            _effect.Parameters["ProjMatrix"].SetValue(Matrix.CreateOrthographicOffCenter(0, vp.Width, vp.Height, 0, 0, -1));
            _effect.Parameters["MainTexture"].SetValue(Texture);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, _effect);
            
            spriteBatch.Draw(Texture, locationSize, null, ColorTint.Value);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        /// <summary>
        /// Get effect parameter.
        /// </summary>
        public EffectParameter GetEffectParameter(string name)
        {
            return _effect?.Parameters.FirstOrDefault(item => item.Name.ToLower() == name.ToLower());
        }        
    }
}
