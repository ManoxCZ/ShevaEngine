using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Noesis;
using ShevaEngine.Core;
using System.Collections.Generic;

namespace ShevaEngine.NoesisUI
{
    public class Layer
    {
        public bool IsActive { get; set; } = true;
        public View View { get; }
        private object _dataContext;
        public object DataContext
        {
            get => _dataContext;
            set
            {
                _dataContext = value;

                Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

                dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
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
        public Layer(View view)
        {
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
            Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                View.SetSize(width, height);
            });
        }

        /// <summary>
        /// Update input.
        /// </summary>        
        public void UpdateInput(InputState state)
        {
            if (_previousInputState == null)
                _previousInputState = state;

            UpdateMouse(state);

            _previousInputState = state;
        }

        /// <summary>
        /// Update mouse.
        /// </summary>        
        private void UpdateMouse(InputState state)
        {
            if (_previousInputState.MouseState.LeftButton == ButtonState.Released &&
                state.MouseState.LeftButton == ButtonState.Pressed)
                View.MouseButtonDown(state.MouseState.X, state.MouseState.Y, MouseButton.Left);

            if (_previousInputState.MouseState.LeftButton == ButtonState.Pressed &&
                state.MouseState.LeftButton == ButtonState.Released)
                View.MouseButtonUp(state.MouseState.X, state.MouseState.Y, MouseButton.Left);

            View.MouseMove(state.MouseState.X, state.MouseState.Y);            
        }
    }
}
