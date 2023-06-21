using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Noesis;
using ShevaEngine.Core;
using ShevaEngine.Core.Profiler;
using ShevaEngine.Core.UI;
using System;
using System.Collections.Generic;

namespace ShevaEngine.NoesisUI;

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


    public Layer()
    {
        RunOnUIThread(() =>
        {
            _view = GUI.CreateView(new U());

#if WINDOWSDX
            RenderDeviceD3D11 device = new(
                ((SharpDX.Direct3D11.Device)ShevaGame.Instance.GraphicsDevice.Handle).ImmediateContext.NativePointer, false);
#elif DESKTOPGL
            RenderDeviceGL device = NoesisUIWrapper.Device;
#endif                        
            _view.Renderer.Init(device);
            
            _view.SetFlags(RenderFlags.LCD | RenderFlags.PPAA);
        });
    }


    public void Update(GameTime time)
    {
        using var profilerScope = ShevaServices.GetService<ProfilerService>().BeginScope(typeof(U).Name);

        _view.Update(time.TotalGameTime.TotalSeconds);
    }

    public void Draw(GameTime time)
    {
        using var profilerScope = ShevaServices.GetService<ProfilerService>().BeginScope(typeof(U).Name);

#if WINDOWSDX
        using (D3X11RenderState _ = new(ShevaGame.Instance.GraphicsDevice))
#elif DESKTOPGL
        
#endif
        {
            _view.Renderer.UpdateRenderTree();

            _view.Renderer.RenderOffscreen();
        }

        foreach (Viewport viewport in GetChildrenOfType<Viewport>(_view.Content))
        {
            viewport.Render(time);
        }

#if WINDOWSDX
        using (D3X11RenderState _ = new(ShevaGame.Instance.GraphicsDevice))
#elif DESKTOPGL
        
#endif
        {
            _view.Renderer.Render();
        }
    }

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

    public void OnWindowResize(int width, int height)
    {
        RunOnUIThread(() =>
        {
            _view.SetSize(width, height);
        });
    }

    public bool UpdateInput(InputState state)
    {
        bool eventHandled = false;

        if (_previousInputState == null)
        {
            _previousInputState = state;
        }

        eventHandled = eventHandled || UpdateMouse(state);

        _previousInputState = state;

        return eventHandled;
    }

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
   
    public void RunOnUIThread(Action action)
    {
        ShevaGame.Instance.SynchronizationContext.Send(_ => action(), null);
    }
}
