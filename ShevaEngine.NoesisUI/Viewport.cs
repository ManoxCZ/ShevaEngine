using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Noesis;
using ShevaEngine.Core;

namespace ShevaEngine.NoesisUI
{
    public sealed class Viewport : UserControl
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
        private RenderTarget2D _renderTarget;
        private RenderTarget2D _depthTarget;
        private object _lock = new object();
        private readonly Image _image;


        /// <summary>
        /// Constructor.
        /// </summary>
        public Viewport()
        {
            _image = new Image()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            Content = _image;
        }                

        /// <summary>
        /// Arrange override.
        /// </summary>
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
                    Name = nameof(Viewport) + "- Render target"
                };

                System.Reflection.FieldInfo info = typeof(RenderTarget2D).GetField("_texture", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                SharpDX.Direct3D11.Resource handle = info.GetValue(_renderTarget) as SharpDX.Direct3D11.Resource;

                _image.Source = new TextureSource(Noesis.Texture.WrapD3D11Texture(_renderTarget, handle.NativePointer, _renderTarget.Width, _renderTarget.Height, 1, false));

                _depthTarget?.Dispose();

                _depthTarget = new RenderTarget2D(
                        ShevaGame.Instance.GraphicsDevice,
                        (int)finalSize.Width,
                        (int)finalSize.Height,
                        false,
                        SurfaceFormat.Single,
                        DepthFormat.None)
                {
                    Name = nameof(Viewport) + "- Depth render target"
                };
            }

            if (Camera != null)
                Camera.Size = new Resolution((int)finalSize.Width, (int)finalSize.Height);

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Draw method.
        /// </summary>        
        public void Render(GameTime gameTime)
        {
            lock (_lock)            
                Camera?.Draw(Scene, gameTime, _renderTarget, _depthTarget);            
        }
    }
}
