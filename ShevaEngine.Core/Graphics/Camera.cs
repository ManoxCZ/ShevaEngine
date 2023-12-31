using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.Profiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShevaEngine.Core
{

    public enum CameraType
    {
        Perspective,
        Orthographic
    }

    /// <summary>
    /// Camera.
    /// </summary> 
    public class Camera : IDisposable
    {
        public CameraType _cameraType;
        public CameraType CameraType
        {
            get => _cameraType;
            set
            {
                _cameraType = value;

                CreateProjectionMatrix();
            }
        }
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        private float _fieldOfView = (float)(Math.PI / 4);
        public float FieldOfView
        {
            get => _fieldOfView;
            set
            {
                _fieldOfView = value;

                CreateProjectionMatrix();
            }
        }
        private Resolution _size = new Resolution(512, 512);
        public Resolution Size
        {
            get => _size;
            set
            {
                _size = value;

                CreateProjectionMatrix();
            }
        }
        private (float Width, float Height) _orthographicSize = (10, 10);
        public (float Width, float Height) OrthographicSize
        {
            get => _orthographicSize;
            set
            {
                _orthographicSize = value;

                CreateProjectionMatrix();
            }
        }
        private float _nearPlane = 1f;
        public float NearPlane
        {
            get => _nearPlane;
            set
            {
                _nearPlane = value;

                CreateProjectionMatrix();
            }
        }
        private float _farPlane = 100f;
        public float FarPlane
        {
            get => _farPlane;
            set
            {
                _farPlane = value;

                CreateProjectionMatrix();
            }
        }

        public Color ClearColor
        {
            get => Color.FromNonPremultiplied(ClearValue);
            set => ClearValue = value.ToVector4();
        }
        public Vector4 ClearValue { get; set; } = new Vector4(0, 0, 0, 0);
        private readonly RenderingPipeline _pipeline;

        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }
        public List<PostProcess> PostProcesses { get; private set; } = new List<PostProcess>();
        public BlendState BlendState
        {
            get => _pipeline.BlendState;
            set
            {
                _pipeline.BlendState = value;
            }
        }
        public RasterizerState RasterizerState
        {
            get => _pipeline.RasterizerState;
            set
            {
                _pipeline.RasterizerState = value;
            }
        }
        public DepthStencilState DepthStencilState
        {
            get => _pipeline.DepthStencilState;
            set
            {
                _pipeline.DepthStencilState = value;
            }
        }
        private RenderTarget2D? _postProcessTarget;
        private SpriteBatch _spriteBatch;
        private bool _saveScreen = false;


        /// <summary>
        /// Constructor.
        /// </summary>
        public Camera(GraphicsDevice graphicsDevice, MaterialProfile matProfile = MaterialProfile.Default)
            : base()
        {
            ViewMatrix = Matrix.Identity;
            CameraType = CameraType.Perspective;            

            lock (graphicsDevice)
            {
                _spriteBatch = new SpriteBatch(graphicsDevice);
            }
            
            _pipeline = new RenderingPipeline("Camera pipeline")
            {
                Profile = matProfile,
            };
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            _spriteBatch.Dispose();
        }

        /// <summary>
        /// Look at.
        /// </summary>
        public void LookAt(Vector3 position, Vector3 target, Vector3 up)
        {
            Position = position;
            Target = target;
            Up = up;

            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
        }

        /// <summary>
        /// Create matrices.
        /// </summary>        
        private void CreateProjectionMatrix()
        {
            switch (CameraType)
            {
                case CameraType.Perspective:
                    ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(FieldOfView, Size.Width / (float)Size.Height, NearPlane, FarPlane);
                    break;
                case CameraType.Orthographic:
                    ProjectionMatrix = Matrix.CreateOrthographic(OrthographicSize.Width, OrthographicSize.Height, NearPlane, FarPlane);
                    break;
            }
        }

        /// <summary>
        /// Draw.
        /// </summary>
        public void Draw(GraphicsDevice graphicsDevice, IScene scene, GameTime gameTime, RenderTarget2D renderTarget, RenderTarget2D? depthTarget = null)
        {
            // Get visible objects.
            _pipeline.Clear();
            _pipeline.GameTime = gameTime;
            _pipeline.SetCamera(this);

            foreach (Light light in scene.GetLights())
                _pipeline.AddLight(light);

            scene.GetVisibleObjects(_pipeline);

            // Update shadows.
            if (_pipeline.Profile == MaterialProfile.Default)
            {
                foreach (Light light in _pipeline.Lights)
                {
                    light.Shadow?.Update(gameTime, scene, light, this);
                }
            }

            // Attach render targets.			
            graphicsDevice.SetRenderTarget(renderTarget);

            graphicsDevice.Clear(
                ClearOptions.Target | ClearOptions.DepthBuffer, ClearValue, 1, 0);

            if (depthTarget != null)
            {
                graphicsDevice.SetRenderTarget(depthTarget);

                graphicsDevice.Clear(
                    ClearOptions.Target, new Vector4(float.MaxValue, 0.0f, 0.0f, 0.0f), 1, 0);

                graphicsDevice.SetRenderTargets(new[]
                {
                    new RenderTargetBinding(renderTarget),
                    new RenderTargetBinding(depthTarget)
                });
            }

            // Render scene.
            _pipeline.Draw();

            graphicsDevice.SetRenderTarget(null);

            // Apply post processes.
            if (PostProcesses.Count > 0)
            {
                if (_postProcessTarget == null ||
                    _postProcessTarget.Width != renderTarget.Width ||
                    _postProcessTarget.Height != renderTarget.Height)
                {
                    _postProcessTarget?.Dispose();

                    _postProcessTarget = new RenderTarget2D(
                        graphicsDevice,
                        renderTarget.Width,
                        renderTarget.Height,
                        false,
                        renderTarget.Format,
                        DepthFormat.None);
                }

                bool postProcessAsTarget = true;

                if (_saveScreen)
                    SaveImage(renderTarget, "Pure image");

                foreach (PostProcess postProcess in PostProcesses)
                {
                    using var _ = ShevaServices.GetService<ProfilerService>().BeginScope(postProcess.GetType().Name);

                    if (postProcess.Enabled)
                    {
                        if (postProcessAsTarget)
                        {
                            postProcess.InputTexture = renderTarget;
                            postProcess.DepthTexture = depthTarget;

                            graphicsDevice.SetRenderTarget(_postProcessTarget);
                        }
                        else
                        {
                            postProcess.InputTexture = _postProcessTarget;
                            postProcess.DepthTexture = depthTarget;

                            graphicsDevice.SetRenderTarget(renderTarget);
                        }

                        graphicsDevice.Clear(
                            ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1, 0);

                        postProcess.Apply(this, gameTime, scene);

                        graphicsDevice.SetRenderTarget(null);

                        if (_saveScreen)
                        {
                            if (postProcessAsTarget)
                                SaveImage(_postProcessTarget, postProcess.GetType().Name.ToString());
                            else
                                SaveImage(renderTarget, postProcess.GetType().Name.ToString());
                        }

                        postProcessAsTarget = !postProcessAsTarget;
                    }
                }

                if (PostProcesses.Where(item => item.Enabled).Count() % 2 != 0)
                {
                    graphicsDevice.SetRenderTarget(renderTarget);

                    _spriteBatch.Begin();
                    _spriteBatch.Draw(_postProcessTarget, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);
                    _spriteBatch.End();

                    graphicsDevice.SetRenderTarget(null);
                }
            }            

            if (_saveScreen)
            {
                _saveScreen = false;

                SaveImage(renderTarget);
            }
        }

        /// <summary>
        /// Save image.
        /// </summary>
        private void SaveImage(RenderTarget2D target, string? name = null)
        {
            if (System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name is string assemblyName)
            {
                string directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    assemblyName,
                    "Screens");

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string realName = name == null ? $"screen{DateTime.Now.Ticks}.png" : name + ".png";

                using (Stream stream = File.Create(Path.Combine(directory, realName)))
                    target.SaveAsPng(stream, target.Width, target.Height);
            }
        }
    }
}
