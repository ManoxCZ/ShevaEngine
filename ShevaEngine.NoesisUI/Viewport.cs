using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Noesis;
using ShevaEngine.Core;
using System;

namespace ShevaEngine.NoesisUI
{
    public sealed class Viewport : Canvas
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
            : base()
        {
            _image = new Image()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Loaded += (object sender, RoutedEventArgs args) => { Children.Add(_image);

                //Nullable<Noesis.Point> dragStart = null;

                //MouseButtonEventHandler mouseDown = (sender, args) => {
                //    var element = (UIElement)sender;
                //    dragStart = args.GetPosition(element);
                //    element.CaptureMouse();
                //};
                //MouseButtonEventHandler mouseUp = (sender, args) => {
                //    var element = (UIElement)sender;
                //    dragStart = null;
                //    element.ReleaseMouseCapture();
                //};
                //MouseEventHandler mouseMove = (sender, args) => {
                //    if (dragStart != null && args.LeftButton == MouseButtonState.Pressed)
                //    {
                //        var element = (UIElement)sender;
                //        var p2 = args.GetPosition(this);
                //        Canvas.SetLeft(element, p2.X - dragStart.Value.X);
                //        Canvas.SetTop(element, p2.Y - dragStart.Value.Y);
                //    }
                //};
                //Action<UIElement> enableDrag = (element) => {
                //    element.MouseDown += mouseDown;
                //    element.MouseMove += mouseMove;
                //    element.MouseUp += mouseUp;
                //};
                
                //foreach (var shape in Children)
                //{
                //    enableDrag(shape);                    
                //}
            };
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

                _image.Source = new TextureSource(
                    RenderDeviceD3D11.WrapTexture(_renderTarget, handle.NativePointer, _renderTarget.Width, _renderTarget.Height, _renderTarget.LevelCount, false, true));

                _image.Width = finalSize.Width;
                _image.Height = finalSize.Height;

                Canvas.SetTop(_image, 0);
                Canvas.SetLeft(_image, 0);
                
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
