using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.Settings;
using ShevaEngine.Core.UI;
using System;
using System.Collections.Generic;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Game component.
    /// </summary>
    public abstract class ShevaGameComponent : IDisposable
    {
        protected readonly ILogger Log;
        public List<ILayer> Layers { get; } = new();
        public bool IsInitialized { get; private set; } = false;
        public bool IsContentLoaded { get; private set; } = false;
        protected List<IDisposable> Disposables { get; } = new();


        /// <summary>
        /// Constructor.
        /// </summary>
        public ShevaGameComponent()
        {
            Log = ShevaServices.GetService<ILoggerFactory>().CreateLogger(GetType());
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (IDisposable item in Disposables)
            {
                item.Dispose();
            }
        }

        /// <summary>
        /// Initialize method.
        /// </summary>
        public virtual void Initialize(ShevaGame game)
        {
            IsInitialized = true;

            if (ShevaServices.GetService<SettingsService>().GetSettings<GameSettings>() is GameSettings gameSettings)
            {
                Disposables.Add(gameSettings.Resolution.Subscribe(item =>
                {
                    foreach (ILayer layer in Layers)
                    {
                        layer.OnWindowResize(item.Width, item.Height);
                    }
                }));
            }
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
            if (ShevaServices.GetService<SettingsService>().GetSettings<GameSettings>() is GameSettings gameSettings)
            {
                foreach (ILayer layer in Layers)
                {
                    layer.OnWindowResize(gameSettings.Resolution.Value.Width, gameSettings.Resolution.Value.Height);
                }
            }
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
            bool eventHandled = false;

            for (int i = Layers.Count - 1; i >= 0; i--)
            {
                ILayer layer = Layers[i];

                if (layer.IsActive)
                {
                    if (!eventHandled)
                    { 
                        eventHandled = eventHandled || layer.UpdateInput(inputState);
                    }

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
            {
                if (Layers[i].IsActive)
                {
                    Layers[i].Draw(gameTime);
                }
            }
        }
    }
}
