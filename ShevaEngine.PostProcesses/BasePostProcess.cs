using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;


namespace ShevaEngine.PostProcesses
{
    /// <summary>Base post process.</summary>
    public abstract class  BasePostProcess : PostProcess
    {
        protected abstract string EffectFilename { get; }
        private SpriteBatch _spriteBatch;        
        protected Effect Effect;
        public SpriteSortMode SortMode { get; set; } = SpriteSortMode.Immediate;
        public BlendState Blend { get; set; } = BlendState.Opaque;
        public SamplerState Sampler { get; set; } = SamplerState.AnisotropicClamp;        
        public virtual bool UsesVertexShader => false;

        
        /// <summary>
        /// Load content.
        /// </summary>        
        public override void LoadContent(ContentManager content)
        {
            _spriteBatch = new SpriteBatch(ShevaGame.Instance.GraphicsDevice);

            Effect = content.Load<Effect>(Path.Combine("Content", "Effects", "PostProcesses", EffectFilename));
        }

        /// <summary>
		/// Apply post process.
		/// </summary>				
		public override void Apply(Camera camera, GameTime time, Scene scene)
        {
            if (Enabled)
            {
                _spriteBatch.Begin(SortMode, Blend, Sampler, DepthStencilState.None, RasterizerState.CullCounterClockwise);
                
                PrepareEffect(camera, time, scene);
                
                foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    _spriteBatch.Draw(InputTexture, new Rectangle(
                        0, 0, 
                        _spriteBatch.GraphicsDevice.Viewport.Width, _spriteBatch.GraphicsDevice.Viewport.Height),
                        Color.White);
                }
                _spriteBatch.End();
            }
        }        

        /// <summary>
        /// Prepare effect.
        /// </summary>
        protected virtual void PrepareEffect(Camera camera, GameTime time, Scene scene)
        {

        }
    }
}
