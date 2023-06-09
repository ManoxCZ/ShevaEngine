using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Noesis;
using ShevaEngine.Core;
using ShevaEngine.Core.Profiler;
using ShevaEngine.Core.UI;
using System;
using System.Reflection;

namespace ShevaEngine.NoesisUI;

public sealed class Viewport : Grid, IViewport
{
    public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
        nameof(Camera), typeof(Camera), typeof(Viewport), new PropertyMetadata(null));

    public static readonly DependencyProperty SceneProperty = DependencyProperty.Register(
        nameof(Scene), typeof(IScene), typeof(Viewport), new PropertyMetadata(null));

    public static readonly DependencyProperty MouseMoveCommandProperty = DependencyProperty.Register(
        nameof(MouseMoveCommand), typeof(RelayCommand<MouseEventArgs>), typeof(Viewport), new PropertyMetadata(null));

    public static readonly DependencyProperty MouseWheelCommandProperty = DependencyProperty.Register(
        nameof(MouseWheelCommand), typeof(RelayCommand<MouseWheelEventArgs>), typeof(Viewport), new PropertyMetadata(null));

    public static readonly DependencyProperty MouseButtonClickCommandProperty = DependencyProperty.Register(
        nameof(MouseButtonClickCommand), typeof(RelayCommand<MouseButtonEventArgs>), typeof(Viewport), new PropertyMetadata(null));

    public Camera Camera
    {
        get => (Camera)GetValue(CameraProperty);
        set => SetValue(CameraProperty, value);
    }
    
    public IScene Scene
    {
        get => (IScene)GetValue(SceneProperty);
        set => SetValue(SceneProperty, value);
    }

    public RelayCommand<MouseEventArgs>? MouseMoveCommand
    {
        get => (RelayCommand<MouseEventArgs>?)GetValue(MouseMoveCommandProperty);
        set => SetValue(MouseMoveCommandProperty, value);
    }

    public RelayCommand<MouseWheelEventArgs>? MouseWheelCommand
    {
        get => (RelayCommand<MouseWheelEventArgs>?)GetValue(MouseWheelCommandProperty);
        set => SetValue(MouseWheelCommandProperty, value);
    }

    public RelayCommand<MouseButtonEventArgs>? MouseButtonClickCommand
    {
        get => (RelayCommand<MouseButtonEventArgs>?)GetValue(MouseButtonClickCommandProperty);
        set => SetValue(MouseButtonClickCommandProperty, value);
    }

    private RenderTarget2D _renderTarget = null!;
    private RenderTarget2D _depthTarget = null!;
    private object _lock = new();
    private readonly Image _image;


    public Viewport()
        : base()
    {
        _image = new Image()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        Loaded += (object sender, RoutedEventArgs args) =>
        {
            Children.Add(_image);
        };

        MouseMove += (sender, args) => MouseMoveCommand?.Execute(args);
        MouseWheel += (sender, args) => MouseWheelCommand?.Execute(args);
        MouseLeftButtonUp += (sender, args) => MouseButtonClickCommand?.Execute(args);
        MouseLeftButtonDown += (sender, args) => MouseButtonClickCommand?.Execute(args);
        MouseRightButtonUp += (sender, args) => MouseButtonClickCommand?.Execute(args);
        MouseRightButtonDown += (sender, args) => MouseButtonClickCommand?.Execute(args);
    }    

    protected override Size ArrangeOverride(Size finalSize)
    {
        lock (_lock)
        {
            _renderTarget?.Dispose();

            _renderTarget = new RenderTarget2D(
                    ShevaGame.Instance.GraphicsDevice,
                    (int)finalSize.Width,
                    (int)finalSize.Height,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.Depth16,
                    0,
                    RenderTargetUsage.PreserveContents,
                    false)
            {
                Name = $"{nameof(Viewport)} - Render target"
            };

#if WINDOWSDX
            if (typeof(RenderTarget2D).GetField("_renderTargetViews", BindingFlags.Instance | BindingFlags.NonPublic) is FieldInfo info &&
                info.GetValue(_renderTarget) is SharpDX.Direct3D11.RenderTargetView[] targets &&
                targets.Length > 0 &&
                targets[0].Resource is SharpDX.Direct3D11.Resource handle)
            {

                _image.Source = new TextureSource(
                    RenderDeviceD3D11.WrapTexture(
                        _renderTarget, 
                        handle.NativePointer, 
                        _renderTarget.Width, 
                        _renderTarget.Height, 
                        1, 
                        false,
                        true));

                _image.Width = finalSize.Width;
                _image.Height = finalSize.Height;

                _depthTarget?.Dispose();

                _depthTarget = new RenderTarget2D(
                        ShevaGame.Instance.GraphicsDevice,
                        (int)finalSize.Width,
                        (int)finalSize.Height,
                        false,
                        SurfaceFormat.Single,
                        DepthFormat.None)
                {
                    Name = $"{nameof(Viewport)} - Depth render target"
                };
            }
#elif DESKTOPGL
            throw new NotImplementedException();
#endif
        }

        if (Camera != null)
        {
            Camera.Size = new Resolution((int)finalSize.Width, (int)finalSize.Height);
        }

        return base.ArrangeOverride(finalSize);
    }    

    public void Render(GameTime gameTime)
    {
        using var _ = ShevaServices.GetService<ProfilerService>().BeginScope(Name);
        
        lock (_lock)
        {
            Camera?.Draw(ShevaGame.Instance.GraphicsDevice, Scene, gameTime, _renderTarget, _depthTarget);
        }
    }
}
