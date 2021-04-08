using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Gui manager.
    /// </summary>	
    public class Layer : IDisposable
    {        
        public Control Control { get; set; }
		public BehaviorSubject<bool> IsActive { get; }		
		public ReplaySubject<(InputState InputState, EventContext Context)> MouseMove { get; }
		public List<IDisposable> Disposables { get; private set; }
		public BehaviorSubject<bool> IsEventBlocking { get; }
        public LayerSelection Selection { get; }
        public BehaviorSubject<ModelView> DataContext => Control?.DataContext;


		/// <summary>
		/// Constructor.
		/// </summary>
		public Layer()
		{
			Control = new Grid();
			Disposables = new List<IDisposable>();
			IsActive = new BehaviorSubject<bool>(false);
			MouseMove = new ReplaySubject<(InputState InputState, EventContext Context)>();
			IsEventBlocking = new BehaviorSubject<bool>(false);
            Selection = new LayerSelection(this);


			Disposables.Add(
				MouseMove.CombineLatest(IsActive, (input, isActive) => (input, isActive))
					.Subscribe(item =>
					{
						if (item.isActive)
						{
							if (Control != null)
								item.input.Context.Processed = Control.OnMouseMove(item.input.InputState);

							if (IsEventBlocking.Value)
								item.input.Context.Processed = true;
						}
					}));
		}
		
		/// <summary>
		/// Dispose.
		/// </summary>
		public virtual void Dispose()
		{
			IsActive?.Dispose();
			MouseMove?.Dispose();
			IsEventBlocking?.Dispose();            

			foreach (IDisposable disposable in Disposables)			
				disposable.Dispose();			
		}
        
		/// <summary>
		/// Resize.
		/// </summary>        
		public void OnWindowResize(int width, int height)
        {
            Control?.Resize(new Rectangle(0, 0, width, height));
        }

        /// <summary>
        /// On mouse click.
        /// </summary>        
        public bool OnMouseClick(InputState inputState) 
        {
			bool result = false;
			
			if (Control != null)
				result = Control.OnMouseClick(inputState);

			if (IsEventBlocking.Value)
				result = true;

			return result;
        }
		
		/// <summary>
		/// On mouse move.
		/// </summary>        
		public bool OnMouseWheel(InputState inputState)
		{
			bool result = false;
			
			if (Control != null)
				result = Control.OnMouseWheel(inputState);

			if (IsEventBlocking.Value)
				result = true;

			return result;			
		}

		/// <summary>
		/// Method draw UI.
		/// </summary>
		public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Control?.Draw(spriteBatch, gameTime);
        }        	
        
        /// <summary>
        /// Get selectable controls.
        /// </summary>
        /// <returns></returns>
        public void GetSelectableControls(List<Control> controls)
        {
            controls.Clear();

            Control?.GetSelectableControls(controls);
        }

        /// <summary>
        /// Get control.
        /// </summary>
        public Control GetControl(string name)
        {
            return Control?.GetControl(name);
        }
    }
}
