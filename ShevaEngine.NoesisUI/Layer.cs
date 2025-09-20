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

    private InputState _previousInputState;


    public Layer()
    {
        RunOnUIThread(() =>
        {
            _view = GUI.CreateView(new U());            

            _view.SetFlags(RenderFlags.LCD | RenderFlags.PPAA);

            _view.Renderer.Init(NoesisUIWrapper.Device);
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

        _view.Renderer.UpdateRenderTree();

        _view.Renderer.RenderOffscreen();

        foreach (Viewport viewport in GetChildrenOfType<Viewport>(_view.Content))
        {
            viewport.Render(time);
        }

        _view.Renderer.Render();
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

            _view.Update(0);

            _view.Renderer.UpdateRenderTree();
        });
    }

    public bool UpdateInput(in InputState state)
    {
        bool eventHandled = false;        

        eventHandled = eventHandled || UpdateMouse(state);

        _previousInputState = state;

        return eventHandled;
    }

    private bool UpdateMouse(in InputState state)
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

    public bool UpdateKeyUpEvent(Keys key)
    {
        return _view.KeyUp(Noesis.MonoGame.KeyConverter.Convert(key));
    }

    public bool UpdateKeyDownEvent(Keys key)
    {
        return _view.KeyDown(Noesis.MonoGame.KeyConverter.Convert(key));
    }

    public bool UpdateInputTextEvent(char key)
    {
        return _view.Char(key);
    }
    
    public void RunOnUIThread(Action action)
    {
        ShevaGame.Instance.SynchronizationContext.Send((_) => action(), null);
    }    
}
