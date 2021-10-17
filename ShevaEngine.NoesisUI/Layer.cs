using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Noesis;
using ShevaEngine.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShevaEngine.NoesisUI
{
    public class Layer
    {
        public ShevaGame Game { get; }
        public bool IsActive { get; set; } = true;
        public View View { get; }
        private object _dataContext;
        public object DataContext
        {
            get => _dataContext;
            set
            {
                _dataContext = value;

                Game.TasksManager.RunTaskOnMainThread(new Task(() =>
                {
                    if (View.Content != null)
                        View.Content.DataContext = DataContext;
                }));
            }
        }
        private InputState _previousInputState;


        /// <summary>
        /// Constructor.
        /// </summary>        
        public Layer(ShevaGame game, View view)
        {
            Game = game;
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
            View.Renderer.UpdateRenderTree();

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
            Game.TasksManager.RunTaskOnMainThread(new Task(() =>
            {
                View.SetSize(width, height);
            }));
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
        /// Get element.
        /// </summary>
        public Task<FrameworkElement> GetElement(string name)
        {
            Task<FrameworkElement> task = new Task<FrameworkElement>(() =>            
            {             
                return View?.Content?.FindName(name) as FrameworkElement;
            });

            Game.TasksManager.RunTaskOnMainThread(task);

            return task;
        }
    }
}
