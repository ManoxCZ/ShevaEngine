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
    public abstract class ShevaGameComponent : ViewModel, IDisposable
    {        
        private readonly List<ILayer> _layers = new();
        public bool IsInitialized { get; private set; } = false;
        public bool IsContentLoaded { get; private set; } = false;
     

        public ShevaGameComponent()
        {            
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
                    foreach (ILayer layer in _layers)
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

        public void AddLayer(ILayer layer)
        {
            if (!_layers.Contains(layer))
            {
                _layers.Add(layer);

                if (ShevaServices.GetService<SettingsService>().GetSettings<GameSettings>() is GameSettings gameSettings)
                {
                    layer.OnWindowResize(gameSettings.Resolution.Value.Width, gameSettings.Resolution.Value.Height);
                }
            }
        }

        public void RemoveLayer(ILayer layer)
        {
            if (_layers.Contains(layer))
            {
                _layers.Remove(layer);
            }
        }

        /// <summary>
        /// Activate method.
        /// </summary>
        public virtual void Activate(ShevaGame game)
        {
            if (ShevaServices.GetService<SettingsService>().GetSettings<GameSettings>() is GameSettings gameSettings)
            {
                foreach (ILayer layer in _layers)
                {
                    layer.OnWindowResize(gameSettings.Resolution.Value.Width, gameSettings.Resolution.Value.Height);
                }
            }

            if (game.Window is GameWindow gameWindow)
            {
                gameWindow.KeyDown += Window_KeyDown;
                gameWindow.KeyUp += Window_KeyUp;
                gameWindow.TextInput += Window_TextInput;
            }
        }        

        /// <summary>
        /// Deactivate method.
        /// </summary>
        public virtual void Deactivate(ShevaGame game)
        {
            if (game.Window is GameWindow gameWindow)
            {
                gameWindow.KeyDown -= Window_KeyDown;
                gameWindow.KeyUp -= Window_KeyUp;
                gameWindow.TextInput -= Window_TextInput;
            }
        }

        private void Window_KeyUp(object? sender, InputKeyEventArgs e)
        {
            bool eventHandled = false;

            for (int i = _layers.Count - 1; i >= 0; i--)
            {
                ILayer layer = _layers[i];

                if (layer.IsActive)
                {
                    if (!eventHandled)
                    {
                        eventHandled = eventHandled || layer.UpdateKeyUpEvent(e.Key);
                    }                    
                }
            }
        }

        private void Window_KeyDown(object? sender, InputKeyEventArgs e)
        {
            bool eventHandled = false;

            for (int i = _layers.Count - 1; i >= 0; i--)
            {
                ILayer layer = _layers[i];

                if (layer.IsActive)
                {
                    if (!eventHandled)
                    {
                        eventHandled = eventHandled || layer.UpdateKeyDownEvent(e.Key);
                    }
                }
            }
        }

        private void Window_TextInput(object? sender, TextInputEventArgs e)
        {
            bool eventHandled = false;

            for (int i = _layers.Count - 1; i >= 0; i--)
            {
                ILayer layer = _layers[i];

                if (layer.IsActive)
                {
                    if (!eventHandled)
                    {
                        eventHandled = eventHandled || layer.UpdateInputTextEvent(e.Character);
                    }
                }
            }
        }

        /// <summary>
        /// Update method.
        /// </summary>		
        public virtual void Update(GameTime time, in InputState inputState)
        {
            bool eventHandled = false;

            for (int i = _layers.Count - 1; i >= 0; i--)
            {
                ILayer layer = _layers[i];

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

            for (int i = 0; i < _layers.Count; i++)
            {
                if (_layers[i].IsActive)
                {
                    _layers[i].Draw(gameTime);
                }
            }
        }
    }
}
