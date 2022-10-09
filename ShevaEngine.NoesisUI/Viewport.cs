using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Noesis;
using ShevaEngine.Core;
using ShevaEngine.Core.UI;
using System.Reflection;

namespace ShevaEngine.NoesisUI;

public sealed class Viewport : Grid, IViewport
{
    public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
        nameof(Camera), typeof(Camera), typeof(Viewport), new PropertyMetadata(null));

    public static readonly DependencyProperty SceneProperty = DependencyProperty.Register(
        nameof(Scene), typeof(IScene), typeof(Viewport), new PropertyMetadata(null));

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
    private RenderTarget2D _renderTarget = null!;
    private RenderTarget2D _depthTarget = null!;
    private object _lock = new object();
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
        }

        if (Camera != null)
            Camera.Size = new Resolution((int)finalSize.Width, (int)finalSize.Height);

        return base.ArrangeOverride(finalSize);
    }

    public void Render(GameTime gameTime)
    {
        lock (_lock)
            Camera?.Draw(Scene, gameTime, _renderTarget, _depthTarget);
    }
}
