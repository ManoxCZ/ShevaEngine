using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.NoesisUI;
using System;
using System.Collections.Generic;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Game component.
    /// </summary>
    public abstract class ShevaGameComponent : IDisposable
    {
		protected readonly Log Log;		
		public List<Layer> Layers { get; } = new List<Layer>();
		public bool IsInitialized { get; private set; } = false;
		public bool IsContentLoaded { get; private set; } = false;
		protected List<IDisposable> Disposables { get; } = new List<IDisposable>();		        


		/// <summary>
		/// Constructor.
		/// </summary>
		public ShevaGameComponent()
		{
			Log = new Log(GetType());			
		}

		/// <summary>
		/// Dispose.
		/// </summary>
		public virtual void Dispose()
		{
			foreach (IDisposable item in Disposables)
				item.Dispose();			
		}

		/// <summary>
		/// Initialize method.
		/// </summary>
		public virtual void Initialize(ShevaGame game)
		{			
			IsInitialized = true;

            Disposables.Add(game.Settings.Resolution.Subscribe(item =>
            {
                foreach (Layer layer in Layers)
                    layer.OnWindowResize(item.Width, item.Height);                
            }));
		}

		/// <summary>
		/// Load content.
		/// </summary>		
		public virtual void LoadContent(ShevaGame game)
		{
			IsContentLoaded = true;
		}

		/// <summary>
		/// Unload content.
		/// </summary>
		public virtual void UnloadContent()
		{

		}

		/// <summary>
		/// Activate method.
		/// </summary>
		public virtual void Activate(ShevaGame game)
		{									
            foreach (Layer layer in Layers)
                layer.OnWindowResize(game.Settings.Resolution.Value.Width, game.Settings.Resolution.Value.Height);            
        }

		/// <summary>
		/// Deactivate method.
		/// </summary>
        public virtual void Deactivate(ShevaGame game)
		{            
		}

		/// <summary>
		/// Update method.
		/// </summary>		
		public virtual void Update(GameTime time, InputState inputState)
		{
            foreach (Layer layer in Layers)
            {
                if (layer.IsActive)
                {
                    layer.UpdateInput(inputState);
                    layer.Update(time);
                }
            }
        }

		/// <summary>
		/// Draw method.
		/// </summary>
		public virtual void Draw(GameTime gameTime)
		{
			ShevaGame.Instance.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Orange, 1, 0);					

            for (int i = 0; i < Layers.Count; i++)
                if (Layers[i].IsActive)
                    Layers[i].Draw(gameTime);                
		}	       
	}
}
