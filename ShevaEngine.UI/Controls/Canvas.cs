using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Canvas.
	/// </summary>	
	public class Canvas : Control
	{		
		public Camera Camera { get; set; }		
		public IScene Scene { get; set; }		
		private RenderTarget2D _renderTarget;
		private RenderTarget2D _depthTarget;
		private object _lock = new object();
        public RenderTarget2D RenderTarget => _renderTarget;


		/// <summary>
		/// Resize.
		/// </summary>
		public override void Resize(Rectangle locationSize)
		{
			base.Resize(locationSize);

			lock (_lock)
			{
				_renderTarget?.Dispose();

				_renderTarget = new RenderTarget2D(
						ShevaGame.Instance.GraphicsDevice,
						LocationSize.Width, 
						LocationSize.Height,
						false,
						SurfaceFormat.Color, 
						DepthFormat.Depth16, 
						0, 
						RenderTargetUsage.PreserveContents,
						false)
                {
                    Name = nameof(Canvas) + "- Render target"
                };

                _depthTarget?.Dispose();

				_depthTarget = new RenderTarget2D(
						ShevaGame.Instance.GraphicsDevice, 
						LocationSize.Width, LocationSize.Height,
						false, 
						SurfaceFormat.Single, 
						DepthFormat.Depth16)
                {
                    Name = nameof(Canvas) + "- Depth render target"
                };
            }

			Camera.Size = new Resolution(locationSize.Width, locationSize.Height);			
		}

		/// <summary>
		/// Draw method.
		/// </summary>        
		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			lock (_lock)
			{
				Camera?.Draw(Scene, gameTime, _renderTarget, _depthTarget);
				
				spriteBatch.Draw(_renderTarget,
					LocationSize,
					new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height),
					Color.White);				
			}
		}
	}
}
