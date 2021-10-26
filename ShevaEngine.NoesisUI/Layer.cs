using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Noesis;
using ShevaEngine.Core;
using ShevaEngine.Core.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShevaEngine.NoesisUI
{
    public class Layer : ILayer
    {
        private readonly NoesisUIWrapper _uiWrapper;
        public bool IsActive { get; set; } = true;
        public View View { get; }
        private object _dataContext;
        public object DataContext
        {
            get => _dataContext;
            set
            {
                _dataContext = value;

                _uiWrapper.RunOnUIThread(() =>                 
                {
                    if (View.Content != null)
                        View.Content.DataContext = DataContext;
                });                
            }
        }
        private InputState _previousInputState;


        /// <summary>
        /// Constructor.
        /// </summary>        
        public Layer(NoesisUIWrapper uiWrapper, View view)
        {
            _uiWrapper = uiWrapper;
            View = view;                             
        }


        public void Update(GameTime time)
        {            
            View.Update(time.TotalGameTime.TotalSeconds);
        }

        /// <summary>
		/// Method draw UI.
		/// </summary>
		public void Draw(GameTime time)
        {
            bool updated = View.Renderer.UpdateRenderTree();

            foreach (Viewport viewport in GetChildrenOfType<Viewport>(View.Content))
                viewport.Render(time);

            View.Renderer.Render();
        }

        /// <summary>
        /// Get children of type.
        /// </summary>
        public static IEnumerable<T> GetChildrenOfType<T>(DependencyObject root)
            where T : DependencyObject
        {
            if (root != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(root, i);

                    if (child is T tInstance)
                        yield return tInstance;

                    foreach (T instance in GetChildrenOfType<T>(child))
                        yield return instance;
                }
            }
        }

        /// <summary>
		/// Resize.
		/// </summary>        
		public void OnWindowResize(int width, int height)
        {
            _uiWrapper.RunOnUIThread(() =>
            {
                View.SetSize(width, height);
            });
        }

        /// <summary>
        /// Update input.
        /// </summary>        
        public bool UpdateInput(InputState state)
        {
            bool eventHandled = false;

            if (_previousInputState == null)
                _previousInputState = state;

            eventHandled = eventHandled || UpdateMouse(state);

            _previousInputState = state;

            return eventHandled;
        }

        /// <summary>
        /// Update mouse.
        /// </summary>        
        private bool UpdateMouse(InputState state)
        {
            bool eventHandled = false;

            if (_previousInputState.MouseState.LeftButton == ButtonState.Released &&
                state.MouseState.LeftButton == ButtonState.Pressed)
                eventHandled = eventHandled || View.MouseButtonDown(state.MouseState.X, state.MouseState.Y, MouseButton.Left);

            if (_previousInputState.MouseState.LeftButton == ButtonState.Pressed &&
                state.MouseState.LeftButton == ButtonState.Released)
                eventHandled = eventHandled || View.MouseButtonUp(state.MouseState.X, state.MouseState.Y, MouseButton.Left);

            if (_previousInputState.MouseState.RightButton == ButtonState.Released &&
                state.MouseState.RightButton == ButtonState.Pressed)
                eventHandled = eventHandled || View.MouseButtonDown(state.MouseState.X, state.MouseState.Y, MouseButton.Right);

            if (_previousInputState.MouseState.RightButton == ButtonState.Pressed &&
                state.MouseState.RightButton == ButtonState.Released)
                eventHandled = eventHandled || View.MouseButtonUp(state.MouseState.X, state.MouseState.Y, MouseButton.Right);

            if (state.MouseState.ScrollWheelValue != _previousInputState.MouseState.ScrollWheelValue)
                eventHandled = eventHandled || View.MouseWheel(state.MouseState.X, state.MouseState.Y, state.MouseState.ScrollWheelValue - _previousInputState.MouseState.ScrollWheelValue);

            if (state.MouseState.HorizontalScrollWheelValue != _previousInputState.MouseState.HorizontalScrollWheelValue)
                eventHandled = eventHandled || View.MouseHWheel(state.MouseState.X, state.MouseState.Y, state.MouseState.HorizontalScrollWheelValue - _previousInputState.MouseState.HorizontalScrollWheelValue);

            if (state.MouseState.X != _previousInputState.MouseState.X ||
                state.MouseState.Y != _previousInputState.MouseState.Y)
                eventHandled = eventHandled || View.MouseMove(state.MouseState.X, state.MouseState.Y);

            return eventHandled;
        }

        /// <summary>
        /// Get viewport.
        /// </summary>
        public Task<IViewport> GetViewport(string name)
        {
            return _uiWrapper.RunFuncOnUIThread(() =>
            {             
                return View?.Content?.FindName(name) as IViewport;
            });           
        }
    }
}
