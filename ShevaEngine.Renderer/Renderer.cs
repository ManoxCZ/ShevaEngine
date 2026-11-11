using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShevaEngine.Renderer
{
    /// <summary>
    /// Main renderer implementation.
    /// This is the new renderer that will replace the legacy RenderingPipeline.
    /// </summary>
    public class Renderer : IRenderer
    {
        private readonly ILogger _log;
        private readonly Dictionary<Effect, Dictionary<ModelMeshPart, List<Matrix>>> _opaqueDrawCalls = new();
        private readonly List<Tuple<Material, ModelMeshPart, Matrix>> _transparentDrawCalls = new();
        private VertexBuffer? _instancesBuffer;
        private int _instancesAllocatedCount = 0;
        private bool _initialized = false;

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice => ShevaGame.Instance.GraphicsDevice;

        /// <summary>
        /// Gets or sets the current camera.
        /// </summary>
        public Camera Camera { get; set; } = null!;

        /// <summary>
        /// Gets or sets the current material profile.
        /// </summary>
        public MaterialProfile Profile { get; set; } = MaterialProfile.Default;

        /// <summary>
        /// Gets the current game time.
        /// </summary>
        public GameTime GameTime { get; set; } = null!;

        /// <summary>
        /// Gets the list of lights in the scene.
        /// </summary>
        public IList<Light> Lights { get; } = new List<Light>();

        /// <summary>
        /// Gets or sets the blend state.
        /// </summary>
        public BlendState BlendState { get; set; } = BlendState.Opaque;

        /// <summary>
        /// Gets or sets the rasterizer state.
        /// </summary>
        public RasterizerState RasterizerState { get; set; } = RasterizerState.CullNone;

        /// <summary>
        /// Gets or sets the depth stencil state.
        /// </summary>
        public DepthStencilState DepthStencilState { get; set; } = DepthStencilState.Default;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Renderer()
        {
            _log = ShevaServices.GetService<ILoggerFactory>().CreateLogger(GetType());
        }

        /// <summary>
        /// Initializes the renderer.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;
            _log.LogInformation("Renderer initialized");
        }

        /// <summary>
        /// Begins a render pass.
        /// </summary>
        public void Begin()
        {
            Clear();
            GraphicsDevice.BlendState = BlendState;
            GraphicsDevice.RasterizerState = RasterizerState;
            GraphicsDevice.DepthStencilState = DepthStencilState;
        }

        /// <summary>
        /// Ends the current render pass.
        /// </summary>
        public void End()
        {
            // Any cleanup if needed
        }

        /// <summary>
        /// Clears the renderer state.
        /// </summary>
        public void Clear()
        {
            ClearLights();
            _transparentDrawCalls.Clear();

            foreach (var effectMeshes in _opaqueDrawCalls)
            {
                foreach (var modelPart in effectMeshes.Value)
                {
                    modelPart.Value.Clear();
                }
            }
        }

        /// <summary>
        /// Clears all lights from the scene.
        /// </summary>
        public void ClearLights()
        {
            Lights.Clear();
        }

        /// <summary>
        /// Adds a light to the scene.
        /// </summary>
        public void AddLight(Light light)
        {
            Lights.Add(light);
        }

        /// <summary>
        /// Adds an object to be rendered.
        /// </summary>
        public void AddObject(Model model, Matrix worldMatrix, AnimationsController? controller = null)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            foreach (ModelMesh mesh in model.Meshes)
            {
                AddObject(mesh, worldMatrix, controller);
            }
        }

        /// <summary>
        /// Adds an object to be rendered.
        /// </summary>
        public void AddObject(ModelMesh mesh, Matrix worldMatrix, AnimationsController? controller = null)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));

            for (int i = 0; i < mesh.MeshParts.Count; i++)
            {
                AddObject(mesh.MeshParts[i], worldMatrix, controller);
            }
        }

        /// <summary>
        /// Adds an object to be rendered.
        /// </summary>
        public void AddObject(ModelMeshPart meshPart, Matrix worldMatrix, AnimationsController? controller = null)
        {
            if (meshPart == null)
                throw new ArgumentNullException(nameof(meshPart));

            if (meshPart.Effect is Material material)
            {
                // Skip if shadow profile and material doesn't cast shadows
                if (Profile == MaterialProfile.Shadows && !material.CastShadows)
                    return;

                // Handle animated materials
                if (material.Animated)
                {
                    if (controller != null)
                    {
                        material.Bones = controller.GetTransforms(GameTime);
                    }
                    else
                    {
                        _log.LogError($"Material {material} is animated, but Animation Controller is null");
                    }
                }

                if (material.Transparent)
                {
                    _transparentDrawCalls.Add(new Tuple<Material, ModelMeshPart, Matrix>(
                        material, meshPart, worldMatrix));
                }
                else
                {
                    if (!_opaqueDrawCalls.ContainsKey(meshPart.Effect))
                    {
                        _opaqueDrawCalls.Add(meshPart.Effect, new Dictionary<ModelMeshPart, List<Matrix>>());
                    }

                    var meshes = _opaqueDrawCalls[meshPart.Effect];
                    if (!meshes.ContainsKey(meshPart))
                    {
                        meshes.Add(meshPart, new List<Matrix>());
                    }

                    meshes[meshPart].Add(worldMatrix);
                }
            }
        }

        /// <summary>
        /// Adds multiple instances of an object to be rendered.
        /// </summary>
        public void AddObject(Model model, IReadOnlyCollection<Matrix> worldMatrices)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            foreach (ModelMesh mesh in model.Meshes)
            {
                AddObject(mesh, worldMatrices);
            }
        }

        /// <summary>
        /// Adds multiple instances of an object to be rendered.
        /// </summary>
        public void AddObject(ModelMesh mesh, IReadOnlyCollection<Matrix> worldMatrices)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));

            for (int i = 0; i < mesh.MeshParts.Count; i++)
            {
                AddObject(mesh.MeshParts[i], worldMatrices);
            }
        }

        /// <summary>
        /// Adds multiple instances of an object to be rendered.
        /// </summary>
        public void AddObject(ModelMeshPart meshPart, IReadOnlyCollection<Matrix> worldMatrices)
        {
            if (meshPart == null)
                throw new ArgumentNullException(nameof(meshPart));

            if (meshPart.Effect is Material material)
            {
                // Skip if shadow profile and material doesn't cast shadows
                if (Profile == MaterialProfile.Shadows && !material.CastShadows)
                    return;

                if (material.Transparent)
                {
                    foreach (Matrix worldMatrix in worldMatrices)
                    {
                        _transparentDrawCalls.Add(new Tuple<Material, ModelMeshPart, Matrix>(
                            material, meshPart, worldMatrix));
                    }
                }
                else
                {
                    if (!_opaqueDrawCalls.ContainsKey(meshPart.Effect))
                    {
                        _opaqueDrawCalls.Add(meshPart.Effect, new Dictionary<ModelMeshPart, List<Matrix>>());
                    }

                    var meshes = _opaqueDrawCalls[meshPart.Effect];
                    if (!meshes.ContainsKey(meshPart))
                    {
                        meshes.Add(meshPart, new List<Matrix>(worldMatrices.Count));
                    }

                    meshes[meshPart].AddRange(worldMatrices);
                }
            }
        }

        /// <summary>
        /// Renders all queued objects.
        /// </summary>
        public void Draw()
        {
            if (Camera == null)
            {
                _log.LogError("Camera is not set");
                return;
            }

            if (GameTime == null)
            {
                _log.LogError("GameTime is not set");
                return;
            }

            try
            {
                GraphicsDevice.BlendState = BlendState;
                GraphicsDevice.RasterizerState = RasterizerState;
                GraphicsDevice.DepthStencilState = DepthStencilState;

                DrawOpaqueObjects();
                DrawTransparentObjects();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        /// <summary>
        /// Draws all opaque objects.
        /// </summary>
        private void DrawOpaqueObjects()
        {
            foreach (var effectMeshes in _opaqueDrawCalls)
            {
                foreach (var modelPart in effectMeshes.Value)
                {
                    if (modelPart.Value.Count > 0)
                    {
                        if (effectMeshes.Key is MaterialWithLights material)
                        {
                            if (Profile != MaterialProfile.Shadows || material.CastShadows)
                            {
                                if (Profile != MaterialProfile.Shadows)
                                    material.Lights = Lights.ToList();

                                material.Apply(Profile, Camera, (float)GameTime.TotalGameTime.TotalSeconds, 
                                    modelPart.Key.VertexBuffer.VertexDeclaration);

                                int instancesCount = UpdateInstancesData(modelPart.Value);

                                GraphicsDevice.SetVertexBuffers(
                                    new VertexBufferBinding(modelPart.Key.VertexBuffer, 0, 0),
                                    new VertexBufferBinding(_instancesBuffer!, 0, 1));
                                GraphicsDevice.Indices = modelPart.Key.IndexBuffer;

                                GraphicsDevice.DrawInstancedPrimitives(
                                    PrimitiveType.TriangleList,
                                    modelPart.Key.VertexOffset,
                                    modelPart.Key.StartIndex,
                                    modelPart.Key.PrimitiveCount,
                                    instancesCount);
                            }
                        }
                        else
                        {
                            _log.LogError($"Invalid material: {effectMeshes.Key.GetType()}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws all transparent objects.
        /// </summary>
        private void DrawTransparentObjects()
        {
            // Don't render transparent objects in shadow pass
            if (Profile == MaterialProfile.Shadows)
                return;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            foreach (var transparentMesh in _transparentDrawCalls
                .OrderByDescending(item => Vector3.DistanceSquared(item.Item3.Translation, Camera.Position)))
            {
                transparentMesh.Item1.Apply(Profile, Camera, (float)GameTime.TotalGameTime.TotalSeconds, 
                    transparentMesh.Item2.VertexBuffer.VertexDeclaration);

                int instancesCount = UpdateInstancesData(new[] { transparentMesh.Item3 });

                GraphicsDevice.SetVertexBuffers(
                    new VertexBufferBinding(transparentMesh.Item2.VertexBuffer, 0, 0),
                    new VertexBufferBinding(_instancesBuffer!, 0, 1));
                GraphicsDevice.Indices = transparentMesh.Item2.IndexBuffer;

                GraphicsDevice.DrawInstancedPrimitives(
                    PrimitiveType.TriangleList,
                    transparentMesh.Item2.VertexOffset,
                    transparentMesh.Item2.StartIndex,
                    transparentMesh.Item2.PrimitiveCount,
                    instancesCount);
            }
        }

        /// <summary>
        /// Updates the instances buffer with world matrices.
        /// </summary>
        private int UpdateInstancesData(IReadOnlyList<Matrix> worldMatrices)
        {
            if (_instancesAllocatedCount < worldMatrices.Count)
            {
                _instancesAllocatedCount = worldMatrices.Count;
                _instancesBuffer?.Dispose();
                _instancesBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexMatrix), _instancesAllocatedCount, BufferUsage.WriteOnly);
            }

            _instancesBuffer.SetData(worldMatrices.ToArray());
            return worldMatrices.Count;
        }

        /// <summary>
        /// Disposes the renderer resources.
        /// </summary>
        public void Dispose()
        {
            _instancesBuffer?.Dispose();
            _instancesBuffer = null;
            _initialized = false;
            _log.LogInformation("Renderer disposed");
        }
    }
}
