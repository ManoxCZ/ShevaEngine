using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShevaEngine.UI;
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
		private SpriteBatch _spriteBatch;
		public List<Layer> Layers { get; } = new List<Layer>();
		public bool IsInitialized { get; private set; } = false;
		public bool IsContentLoaded { get; private set; } = false;
		protected List<IDisposable> Disposables { get; } = new List<IDisposable>();
		private MouseState _previousMouseState;		
		private IDisposable _inputObserver;


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
                {
                    layer.OnWindowResize(item.Width, item.Height);
                }
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
			_spriteBatch = new SpriteBatch(game.GraphicsDevice);			
			
			_inputObserver = game.InputState.Subscribe(item =>
			{
				UpdateInputEvents(ref item);
			});

            foreach (Layer layer in Layers)
                layer.OnWindowResize(game.Settings.Resolution.Value.Width, game.Settings.Resolution.Value.Height);            
        }

		/// <summary>
		/// Deactivate method.
		/// </summary>
        public virtual void Deactivate(ShevaGame game)
		{
			_spriteBatch.Dispose();
			_spriteBatch = null;

			_inputObserver?.Dispose();
			_inputObserver = null;
		}

		/// <summary>
		/// Update method.
		/// </summary>		
		public virtual void Update(GameTime time)
		{			
		}

		/// <summary>
		/// Update input events.
		/// </summary>        
		private void UpdateInputEvents(ref InputState inputState)
		{
			MouseState mouseState = inputState.MouseState;

			if ((mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton != ButtonState.Pressed) ||
				(mouseState.MiddleButton == ButtonState.Pressed && _previousMouseState.MiddleButton != ButtonState.Pressed) ||
				(mouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton != ButtonState.Pressed) ||
				(mouseState.XButton1 == ButtonState.Pressed && _previousMouseState.XButton1 != ButtonState.Pressed) ||
				(mouseState.XButton2 == ButtonState.Pressed && _previousMouseState.XButton2 != ButtonState.Pressed))
			{
				for (int i = Layers.Count - 1; i >= 0; i--)
					if (Layers[i].IsActive.Value)
						if (Layers[i].OnMouseClick(inputState))
							break;
			}

			if (_previousMouseState.Position != mouseState.Position)
			{
				EventContext context = new EventContext();

				for (int i = Layers.Count - 1; i >= 0; i--)
					if (!context.Processed)
						Layers[i].MouseMove.OnNext((inputState, context));							
			}
			
			if (_previousMouseState.ScrollWheelValue != mouseState.ScrollWheelValue)
			{
				for (int i = Layers.Count - 1; i >= 0; i--)
					if (Layers[i].IsActive.Value)
						if (Layers[i].OnMouseWheel(inputState))
							break;
			}
			
			_previousMouseState = mouseState;

			//foreach (TouchLocation touchLocation in inputState.TouchPanelState.GetState())
			//    if (touchLocation.State == TouchLocationState.Pressed)
			//        OnClick((int)touchLocation.Position.X,(int)touchLocation.Position.Y);


		}

		/// <summary>
		/// Draw method.
		/// </summary>
		public virtual void Draw(GameTime gameTime)
		{
			ShevaGame.Instance.GraphicsDevice.Clear(Color.Orange);
			
			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			for (int i = 0; i < Layers.Count; i++)
				if (Layers[i].IsActive.Value)
					Layers[i].Draw(_spriteBatch, gameTime);

			_spriteBatch.End();
		}	
	}
}
