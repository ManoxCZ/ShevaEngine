using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Noesis;
using ShevaEngine.Core;
using ShevaEngine.Core.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShevaEngine.NoesisUI
{
    public class Layer<U> : ILayer where U : UserControl, new()
    {
        public bool IsActive { get; set; } = true;
        private View _view = null!;
        public object DataContext
        {
            get => _view.Content.DataContext;
            set
            {
                RunOnUIThread(() =>
                {
                    if (_view.Content != null)
                    {
                        _view.Content.DataContext = value;
                    }
                });
            }
        }
        private InputState _previousInputState = null!;


        /// <summary>
        /// Constructor.
        /// </summary>        
        public Layer()
        {
            RunOnUIThread(() =>
            {
                _view = GUI.CreateView(new U());

                RenderDeviceD3D11 device = new RenderDeviceD3D11(
                    ((SharpDX.Direct3D11.Device)ShevaGame.Instance.GraphicsDevice.Handle).ImmediateContext.NativePointer, false);

                _view.Renderer.Init(device);
                _view.SetFlags(RenderFlags.LCD | RenderFlags.PPAA);
            });
        }


        public void Update(GameTime time)
        {
            _view.Update(time.TotalGameTime.TotalSeconds);
        }

        /// <summary>
		/// Method draw UI.
		/// </summary>
		public void Draw(GameTime time)
        {
            _view.Renderer.UpdateRenderTree();

            foreach (Viewport viewport in GetChildrenOfType<Viewport>(_view.Content))
                viewport.Render(time);

            _view.Renderer.Render();
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
            RunOnUIThread(() =>
            {
                _view.SetSize(width, height);
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
                eventHandled = eventHandled || _view.MouseButtonDown(state.MouseState.X, state.MouseState.Y, MouseButton.Left);

            if (_previousInputState.MouseState.LeftButton == ButtonState.Pressed &&
                state.MouseState.LeftButton == ButtonState.Released)
                eventHandled = eventHandled || _view.MouseButtonUp(state.MouseState.X, state.MouseState.Y, MouseButton.Left);

            if (_previousInputState.MouseState.RightButton == ButtonState.Released &&
                state.MouseState.RightButton == ButtonState.Pressed)
                eventHandled = eventHandled || _view.MouseButtonDown(state.MouseState.X, state.MouseState.Y, MouseButton.Right);

            if (_previousInputState.MouseState.RightButton == ButtonState.Pressed &&
                state.MouseState.RightButton == ButtonState.Released)
                eventHandled = eventHandled || _view.MouseButtonUp(state.MouseState.X, state.MouseState.Y, MouseButton.Right);

            if (state.MouseState.ScrollWheelValue != _previousInputState.MouseState.ScrollWheelValue)
                eventHandled = eventHandled || _view.MouseWheel(state.MouseState.X, state.MouseState.Y, state.MouseState.ScrollWheelValue - _previousInputState.MouseState.ScrollWheelValue);

            if (state.MouseState.HorizontalScrollWheelValue != _previousInputState.MouseState.HorizontalScrollWheelValue)
                eventHandled = eventHandled || _view.MouseHWheel(state.MouseState.X, state.MouseState.Y, state.MouseState.HorizontalScrollWheelValue - _previousInputState.MouseState.HorizontalScrollWheelValue);

            if (state.MouseState.X != _previousInputState.MouseState.X ||
                state.MouseState.Y != _previousInputState.MouseState.Y)
                eventHandled = eventHandled || _view.MouseMove(state.MouseState.X, state.MouseState.Y);

            return eventHandled;
        }

        /// <summary>
        /// Get viewport.
        /// </summary>
        public Task<IViewport> GetViewport(string name)
        {
            return RunFuncOnUIThread(() =>
            {
                return _view?.Content?.FindName(name) as IViewport;
            });
        }

        /// <summary>
        /// Run on UI thread.
        /// </summary>        
        public void RunOnUIThread(Action action)
        {
            ShevaGame.Instance.SynchronizationContext.Send(_ => action(), null);
        }

        /// <summary>
        /// Run on UI thread.
        /// </summary>        
        public Task<T> RunFuncOnUIThread<T>(Func<T> function)
        {
            TaskCompletionSource<T> taskSource = new TaskCompletionSource<T>();

            ShevaGame.Instance.SynchronizationContext.Send(_ =>
            {
                taskSource.SetResult(function());
            }, null);

            return taskSource.Task;
        }
    }
}
