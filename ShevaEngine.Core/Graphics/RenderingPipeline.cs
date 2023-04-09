using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ShevaEngine.Core
{
    /// <summary>
    /// Rendering pipeline.
    /// </summary>
    public class RenderingPipeline
    {
        private readonly ILogger _log;
        public GraphicsDevice Device => ShevaGame.Instance.GraphicsDevice;
        public string Name { get; }
        public Camera Camera { get; private set; }
        public MaterialProfile Profile { get; set; }
        public GameTime GameTime { get; set; }
        public List<Light> Lights { get; private set; } = new List<Light>();
        private readonly Dictionary<Effect, Dictionary<ModelMeshPart, List<Matrix>>> OpaqueDrawCalls = new();
        private readonly List<DrawCall> _transparentDrawCalls = new();
        private readonly List<Tuple<Material, ModelMeshPart, Matrix>> OldTransparentDrawCalls = new();
        private VertexBuffer _instancesBuffer;        
        private int _instancesAllocatedCount = 0;
        public BlendState BlendState { get; set; } = BlendState.Opaque;
        public RasterizerState RasterizerState { get; set; } = RasterizerState.CullNone;
        public DepthStencilState DepthStencilState { get; set; } = DepthStencilState.Default;



        /// <summary>
        /// Constructor.
        /// </summary>
        public RenderingPipeline(string name)
        {
            Name = name;

            _log = ShevaServices.GetService<ILoggerFactory>().CreateLogger($"{typeof(RenderingPipeline)} - {Name}");

            Profile = MaterialProfile.Default;
        }

        /// <summary>
        /// Clear method.
        /// </summary>
        public void Clear()
        {
            _log.LogDebug("Clear");

            ClearLights();

            _transparentDrawCalls.Clear();
            OldTransparentDrawCalls.Clear();

            foreach (KeyValuePair<Effect, Dictionary<ModelMeshPart, List<Matrix>>> model in OpaqueDrawCalls)
            {
                foreach (KeyValuePair<ModelMeshPart, List<Matrix>> modelPart in model.Value)
                {
                    modelPart.Value.Clear();
                }
            }
        }

        /// <summary>
        /// Clear lights.
        /// </summary>
        public void ClearLights()
        {
            _log.LogDebug("Lights cleared");

            Lights.Clear();
        }

        /// <summary>
        /// Set camera.
        /// </summary>		
        public void SetCamera(Camera camera)
        {
            _log.LogDebug("Set camera");
            Camera = camera;
        }

        /// <summary>
        /// Add light
        /// </summary>		
        public void AddLight(Light light)
        {
            _log.LogDebug("Light added");

            Lights.Add(light);
        }

        /// <summary>
        /// Add object.
        /// </summary>
        public void AddObject(Model model, Matrix worldMatrix, AnimationsController? controller = null)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                AddObject(mesh, worldMatrix, controller);
            }
        }

        /// <summary>
        /// Add object.
        /// </summary>
        public void AddObject(ModelMesh modelPart, Matrix worldMatrix, AnimationsController? controller = null)
        {
            int count = modelPart.MeshParts.Count;

            for (int i = 0; i < count; i++)
            {
                AddObject(modelPart.MeshParts[i], worldMatrix, controller);
            }
        }

        /// <summary>
        /// Add object.
        /// </summary>
        public void AddObject(ModelMeshPart modelMeshPart, Matrix worldMatrix, AnimationsController? controller = null)
        {
            if (modelMeshPart.Effect is Material material)
            {
                if (Profile == MaterialProfile.Shadows && !material.CastShadows)
                {
                    return;
                }

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
                    OldTransparentDrawCalls.Add(new Tuple<Material, ModelMeshPart, Matrix>(
                        material, modelMeshPart, worldMatrix));
                }
                else
                {
                    if (!OpaqueDrawCalls.ContainsKey(modelMeshPart.Effect))
                    {
                        OpaqueDrawCalls.Add(modelMeshPart.Effect, new Dictionary<ModelMeshPart, List<Matrix>>());
                    }

                    Dictionary<ModelMeshPart, List<Matrix>> meshes = OpaqueDrawCalls[modelMeshPart.Effect];

                    if (!meshes.ContainsKey(modelMeshPart))
                    {
                        meshes.Add(modelMeshPart, new List<Matrix>());
                    }

                    meshes[modelMeshPart].Add(worldMatrix);
                }
            }
        }

        /// <summary>
        /// Add object.
        /// </summary>
        public void AddObject(Model model, IReadOnlyCollection<Matrix> worldMatrices)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                AddObject(mesh, worldMatrices);
            }
        }

        /// <summary>
        /// Add object.
        /// </summary>
        public void AddObject(ModelMesh modelPart, IReadOnlyCollection<Matrix> worldMatrices)
        {
            int count = modelPart.MeshParts.Count;

            for (int i = 0; i < count; i++)
            {
                AddObject(modelPart.MeshParts[i], worldMatrices);
            }
        }

        /// <summary>
        /// Add object.
        /// </summary>
        public void AddObject(ModelMeshPart modelMeshPart, IReadOnlyCollection<Matrix> worldMatrices)
        {
            if (modelMeshPart.Effect is ColoredMaterial material)
            {
                if (Profile == MaterialProfile.Shadows && !material.CastShadows)
                {
                    return;
                }

                if (material.Transparent)
                {
                    foreach (Matrix worldMatrix in worldMatrices)
                    {
                        OldTransparentDrawCalls.Add(new Tuple<Material, ModelMeshPart, Matrix>(
                            material, modelMeshPart, worldMatrix));
                    }
                }
                else
                {
                    if (!OpaqueDrawCalls.ContainsKey(modelMeshPart.Effect))
                    {
                        OpaqueDrawCalls.Add(modelMeshPart.Effect, new Dictionary<ModelMeshPart, List<Matrix>>());
                    }

                    Dictionary<ModelMeshPart, List<Matrix>> meshes = OpaqueDrawCalls[modelMeshPart.Effect];

                    if (!meshes.ContainsKey(modelMeshPart))
                    {
                        meshes.Add(modelMeshPart, new List<Matrix>(worldMatrices.Count));
                    }

                    meshes[modelMeshPart].AddRange(worldMatrices);
                }
            }
        }

        /// <summary>
        /// Draw method.
        /// </summary>		
        public void Draw()
        {
            _log.LogDebug($"Draw started with {Profile} profile");

            try
            {
                Device.BlendState = BlendState;
                Device.RasterizerState = RasterizerState;
                Device.DepthStencilState = DepthStencilState;

                foreach (KeyValuePair<Effect, Dictionary<ModelMeshPart, List<Matrix>>> effectMeshes in OpaqueDrawCalls)
                {
                    foreach (KeyValuePair<ModelMeshPart, List<Matrix>> modelPart in effectMeshes.Value)
                    {
                        if (modelPart.Value.Count > 0)
                        {
                            if (effectMeshes.Key is MaterialWithLights material)
                            {
                                if (Profile != MaterialProfile.Shadows || material.CastShadows)
                                {
                                    _log.LogDebug($"Applying material {material}");

                                    if (Profile != MaterialProfile.Shadows)
                                        material.Lights = Lights;

                                    material.Apply(Profile, Camera, (float)GameTime.TotalGameTime.TotalSeconds, modelPart.Key.VertexBuffer.VertexDeclaration);

                                    int instancesCount = UpdateInstancesData(Device, modelPart.Value);

                                    Device.SetVertexBuffers(
                                            new VertexBufferBinding(modelPart.Key.VertexBuffer, 0, 0),
                                            new VertexBufferBinding(_instancesBuffer, 0, 1));
                                    Device.Indices = modelPart.Key.IndexBuffer;

                                    Device.DrawInstancedPrimitives(
                                        PrimitiveType.TriangleList, 
                                        modelPart.Key.VertexOffset, 
                                        modelPart.Key.StartIndex, 
                                        modelPart.Key.PrimitiveCount, 
                                        instancesCount);
                                }
                            }
                            else
                                _log.LogError($"Invalid material: {effectMeshes.Key.GetType()}");
                        }
                    }
                }

                OldDrawTransparentObjects();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }

            _log.LogDebug($"Draw ended");
        }


        private void OldDrawTransparentObjects()
        {
            Device.BlendState = BlendState.AlphaBlend;
            Device.DepthStencilState = DepthStencilState.DepthRead;

            // Transparent materials, don't render to the shadow map.
            if (Profile != MaterialProfile.Shadows)
            {
                foreach (Tuple<Material, ModelMeshPart, Matrix>? transparentMesh in
                    OldTransparentDrawCalls.OrderByDescending(item => Vector3.DistanceSquared(item.Item3.Translation, Camera.Position)))
                {
                    transparentMesh.Item1.Apply(Profile, Camera, GameTime.TotalGameTime.Seconds, transparentMesh.Item2.VertexBuffer.VertexDeclaration);

                    int instancesCount = UpdateInstancesData(Device, new[] { transparentMesh.Item3 });

                    Device.SetVertexBuffers(
                            new VertexBufferBinding(transparentMesh.Item2.VertexBuffer, 0, 0),
                            new VertexBufferBinding(_instancesBuffer, 0, 1));
                    Device.Indices = transparentMesh.Item2.IndexBuffer;

                    Device.DrawInstancedPrimitives(
                        PrimitiveType.TriangleList, 
                        transparentMesh.Item2.VertexOffset,
                        transparentMesh.Item2.StartIndex, 
                        transparentMesh.Item2.PrimitiveCount, 
                        instancesCount);
                }
            }
        }

        private void DrawTransparentObjects()
        {
            Device.BlendState = BlendState.AlphaBlend;
            Device.DepthStencilState = DepthStencilState.DepthRead;

            Matrix[] instances = new Matrix[1];

            // Transparent materials, don't render to the shadow map.
            if (Profile != MaterialProfile.Shadows)
            {
                foreach (DrawCall transparentMesh in
                    _transparentDrawCalls.OrderByDescending(item => Vector3.DistanceSquared(item.Transform.Translation, Camera.Position)))
                {
                    transparentMesh.Item1.Apply(Profile, Camera, GameTime.TotalGameTime.Seconds, transparentMesh.Item2.VertexBuffer.VertexDeclaration);

                    instances[0] = transparentMesh.Transform;
                    int instancesCount = UpdateInstancesData(Device, instances); 

                    Device.SetVertexBuffers(
                            new VertexBufferBinding(transparentMesh.Item2.VertexBuffer, 0, 0),
                            new VertexBufferBinding(_instancesBuffer, 0, 1));
                    Device.Indices = transparentMesh.Item2.IndexBuffer;

                    Device.DrawInstancedPrimitives(
                        PrimitiveType.TriangleList, 
                        transparentMesh.Item2.VertexOffset, 
                        transparentMesh.Item2.StartIndex, 
                        transparentMesh.Item2.PrimitiveCount, 
                        instancesCount);
                }
            }
        }

        /// <summary>
        /// Update instances buffer.
        /// </summary>        
        private int UpdateInstancesData(GraphicsDevice graphicsDevice, IReadOnlyList<Matrix> worldMatrices)
        {
            if (_instancesAllocatedCount < worldMatrices.Count)
            {
                _instancesAllocatedCount = worldMatrices.Count;
                
                _instancesBuffer?.Dispose();

                _instancesBuffer = new VertexBuffer(graphicsDevice, typeof(VertexMatrix), _instancesAllocatedCount, BufferUsage.WriteOnly);
            }

            _instancesBuffer.SetData(worldMatrices.ToArray());
            
            return worldMatrices.Count;
        }
    }
}