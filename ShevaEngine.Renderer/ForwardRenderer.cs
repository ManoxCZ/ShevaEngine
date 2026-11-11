using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.Graphics;
using ShevaEngine.Renderer.Materials;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShevaEngine.Renderer
{
    /// <summary>
    /// Forward renderer implementation for PBR materials.
    /// Renders objects with physically-based lighting and supports:
    /// - Multiple light types (Directional, Point, Spot)
    /// - Shadow mapping
    /// - Image-Based Lighting (IBL)
    /// - Tone mapping and gamma correction
    /// </summary>
    public class ForwardRenderer : IRenderer
    {
        private readonly ILogger _log;
        private readonly Dictionary<Effect, List<RenderItem>> _renderItems = new();
        private VertexBuffer? _instancesBuffer;
        private int _instancesAllocatedCount = 0;
        private bool _initialized = false;

        // Render statistics
        private int _drawCalls = 0;
        private int _triangles = 0;

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
        public RasterizerState RasterizerState { get; set; } = RasterizerState.CullCounterClockwise;

        /// <summary>
        /// Gets or sets the depth stencil state.
        /// </summary>
        public DepthStencilState DepthStencilState { get; set; } = DepthStencilState.Default;

        /// <summary>
        /// Gets the environment texture for IBL.
        /// </summary>
        public TextureCube? EnvironmentTexture { get; set; }

        /// <summary>
        /// Gets or sets the irradiance texture for diffuse IBL.
        /// </summary>
        public TextureCube? IrradianceTexture { get; set; }

        /// <summary>
        /// Gets or sets the BRDF lookup texture.
        /// </summary>
        public Texture2D? BrdfLutTexture { get; set; }

        /// <summary>
        /// Gets or sets the shadow map render target.
        /// </summary>
        public RenderTarget2D? ShadowMap { get; set; }

        /// <summary>
        /// Gets the number of draw calls in the last frame.
        /// </summary>
        public int DrawCalls => _drawCalls;

        /// <summary>
        /// Gets the number of triangles rendered in the last frame.
        /// </summary>
        public int Triangles => _triangles;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ForwardRenderer()
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
            _log.LogInformation("ForwardRenderer initialized");
        }

        /// <summary>
        /// Begins a render pass.
        /// </summary>
        public void Begin()
        {
            if (! _initialized)
                Initialize();

            Clear();
            
            // Set up render states
            GraphicsDevice.BlendState = BlendState;
            GraphicsDevice.RasterizerState = RasterizerState;
            GraphicsDevice.DepthStencilState = DepthStencilState;

            // Clear statistics
            _drawCalls = 0;
            _triangles = 0;
        }

        /// <summary>
        /// Ends the current render pass.
        /// </summary>
        public void End()
        {
            // Nothing to do for now
        }

        /// <summary>
        /// Clears the renderer state.
        /// </summary>
        public void Clear()
        {
            ClearLights();
            _renderItems.Clear();
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

            if (meshPart.Effect == null)
            {
                _log.LogWarning("MeshPart has no effect assigned");
                return;
            }

            var renderItem = new RenderItem
            {
                MeshPart = meshPart,
                WorldMatrix = worldMatrix,
                Controller = controller
            };

            // Group by effect
            if (!_renderItems.ContainsKey(meshPart.Effect))
            {
                _renderItems.Add(meshPart.Effect, new List<RenderItem>());
            }

            _renderItems[meshPart.Effect].Add(renderItem);
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

            if (meshPart.Effect == null)
            {
                _log.LogWarning("MeshPart has no effect assigned");
                return;
            }

            if (!_renderItems.ContainsKey(meshPart.Effect))
            {
                _renderItems.Add(meshPart.Effect, new List<RenderItem>());
            }

            foreach (Matrix matrix in worldMatrices)
            {
                _renderItems[meshPart.Effect].Add(new RenderItem
                {
                    MeshPart = meshPart,
                    WorldMatrix = matrix,
                    Controller = null
                });
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
                // Update matrix parameters for all effects
                foreach (var effectGroup in _renderItems)
                {
                    Effect effect = effectGroup.Key;
                    
                    // Set common parameters
                    SetMatrixParameters(effect);
                    SetLightParameters(effect);
                    SetEnvironmentParameters(effect);
                    SetScreenParameters(effect);
                }

                // Sort render items: opaque first, then transparent
                var opaqueItems = new List<KeyValuePair<Effect, List<RenderItem>>>();
                var transparentItems = new List<KeyValuePair<Effect, List<RenderItem>>>();

                foreach (var effectGroup in _renderItems)
                {
                    // For now, we'll assume all items are opaque
                    // Transparency sorting would need more information from the material
                    opaqueItems.Add(effectGroup);
                }

                // Render opaque items
                GraphicsDevice.BlendState = BlendState;
                GraphicsDevice.RasterizerState = RasterizerState;
                GraphicsDevice.DepthStencilState = DepthStencilState;

                foreach (var effectGroup in opaqueItems)
                {
                    Effect effect = effectGroup.Key;
                    
                    foreach (var renderItem in effectGroup.Value)
                    {
                        DrawRenderItem(renderItem, effect);
                    }
                }

                // Render transparent items (sorted back-to-front)
                if (transparentItems.Count > 0)
                {
                    GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

                    foreach (var effectGroup in transparentItems)
                    {
                        Effect effect = effectGroup.Key;
                        
                        foreach (var renderItem in effectGroup.Value
                            .OrderByDescending(r => Vector3.DistanceSquared(r.WorldMatrix.Translation, Camera.Position)))
                        {
                            DrawRenderItem(renderItem, effect);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        /// <summary>
        /// Sets the matrix parameters for an effect.
        /// </summary>
        private void SetMatrixParameters(Effect effect)
        {
            EffectParameter? viewParam = effect.Parameters.FirstOrDefault(p => p.Name == "ViewMatrix");
            EffectParameter? projParam = effect.Parameters.FirstOrDefault(p => p.Name == "ProjMatrix");
            EffectParameter? invViewParam = effect.Parameters.FirstOrDefault(p => p.Name == "InvViewMatrix");
            EffectParameter? invProjParam = effect.Parameters.FirstOrDefault(p => p.Name == "InvProjMatrix");
            EffectParameter? cameraPosParam = effect.Parameters.FirstOrDefault(p => p.Name == "CameraPosition");
            EffectParameter? gameTimeParam = effect.Parameters.FirstOrDefault(p => p.Name == "GameTime");

            viewParam?.SetValue(Camera.ViewMatrix);
            projParam?.SetValue(Camera.ProjectionMatrix);
            
            if (invViewParam != null)
            {
                Matrix invView;
                Matrix.Invert(ref Camera.ViewMatrix, out invView);
                invViewParam.SetValue(invView);
            }
            
            if (invProjParam != null)
            {
                Matrix invProj;
                Matrix.Invert(ref Camera.ProjectionMatrix, out invProj);
                invProjParam.SetValue(invProj);
            }

            cameraPosParam?.SetValue(Camera.Position);
            gameTimeParam?.SetValue((float)GameTime.TotalGameTime.TotalSeconds);
        }

        /// <summary>
        /// Sets the light parameters for an effect.
        /// </summary>
        private void SetLightParameters(Effect effect)
        {
            EffectParameter? lightsCountParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightsCount");
            EffectParameter? lightTypesParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightTypes");
            EffectParameter? lightPositionsParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightPositions");
            EffectParameter? lightColorsParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightColors");
            EffectParameter? lightViewProjsParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightViewProjs");
            EffectParameter? lightShadowMapSizesParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightShadowMapSizes");
            EffectParameter? shadowMapParam = effect.Parameters.FirstOrDefault(p => p.Name == "Light1ShadowMap");
            EffectParameter? depthBiasParam = effect.Parameters.FirstOrDefault(p => p.Name == "DepthBias");

            int lightCount = Math.Min(Lights.Count, 4);
            lightsCountParam?.SetValue(lightCount);

            if (lightTypesParam != null && lightPositionsParam != null && 
                lightColorsParam != null && lightViewProjsParam != null &&
                lightShadowMapSizesParam != null)
            {
                // Prepare arrays
                float[] lightTypes = new float[4];
                Vector3[] lightPositions = new Vector3[4];
                Vector4[] lightColors = new Vector4[4];
                Matrix[] lightViewProjs = new Matrix[4];
                Vector2[] shadowMapSizes = new Vector2[4];

                for (int i = 0; i < lightCount; i++)
                {
                    Light light = Lights[i];
                    
                    // Light type: 0=Point, 1=Directional, 2=Spot
                    if (light is DirectionalLight)
                    {
                        lightTypes[i] = 1.0f;
                        lightPositions[i] = light.Direction; // Directional lights use position as direction
                    }
                    else
                    {
                        lightTypes[i] = 0.0f; // Point light
                        lightPositions[i] = light.Position;
                    }

                    lightColors[i] = new Vector4(light.Color.ToVector3(), light.Intensity);

                    // Shadow parameters
                    if (light.Shadow != null && light.Shadow is SingleMapShadow singleMapShadow)
                    {
                        lightViewProjs[i] = light.Shadow.GetLightViewProjMatrix();
                        shadowMapSizes[i] = new Vector2(singleMapShadow.Size.Width, singleMapShadow.Size.Height);
                    }
                    else
                    {
                        lightViewProjs[i] = Matrix.Identity;
                        shadowMapSizes[i] = Vector2.Zero;
                    }
                }

                lightTypesParam.SetValue(lightTypes);
                lightPositionsParam.SetValue(lightPositions);
                lightColorsParam.SetValue(lightColors);
                lightViewProjsParam.SetValue(lightViewProjs);
                lightShadowMapSizesParam.SetValue(shadowMapSizes);
            }

            // Set shadow map if available
            if (shadowMapParam != null && ShadowMap != null)
            {
                shadowMapParam.SetValue(ShadowMap);
            }

            depthBiasParam?.SetValue(0.05f);
        }

        /// <summary>
        /// Sets the environment parameters for IBL.
        /// </summary>
        private void SetEnvironmentParameters(Effect effect)
        {
            EffectParameter? enviroParam = effect.Parameters.FirstOrDefault(p => p.Name == "EnviroTexture");
            EffectParameter? irradianceParam = effect.Parameters.FirstOrDefault(p => p.Name == "IrradianceTexture");
            EffectParameter? brdfLutParam = effect.Parameters.FirstOrDefault(p => p.Name == "BrdfLutTexture");
            EffectParameter? envRotationParam = effect.Parameters.FirstOrDefault(p => p.Name == "EnvRotation");

            enviroParam?.SetValue(EnvironmentTexture);
            irradianceParam?.SetValue(IrradianceTexture);
            brdfLutParam?.SetValue(BrdfLutTexture);
            envRotationParam?.SetValue(Vector3.Zero); // TODO: Add environment rotation support
        }

        /// <summary>
        /// Sets the screen and post-processing parameters.
        /// </summary>
        private void SetScreenParameters(Effect effect)
        {
            EffectParameter? screenResParam = effect.Parameters.FirstOrDefault(p => p.Name == "ScreenResolution");
            EffectParameter? exposureParam = effect.Parameters.FirstOrDefault(p => p.Name == "Exposure");
            EffectParameter? gammaParam = effect.Parameters.FirstOrDefault(p => p.Name == "Gamma");

            screenResParam?.SetValue(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            exposureParam?.SetValue(1.0f); // Default exposure
            gammaParam?.SetValue(2.2f); // Default gamma
        }

        /// <summary>
        /// Draws a single render item.
        /// </summary>
        private void DrawRenderItem(RenderItem renderItem, Effect effect)
        {
            try
            {
                ModelMeshPart meshPart = renderItem.MeshPart;
                
                // Handle animated models
                if (renderItem.Controller != null && meshPart.Effect is Material material && material.Animated)
                {
                    material.Bones = renderItem.Controller.GetTransforms(GameTime);
                }

                // Set world transform
                EffectParameter? transformParam = effect.Parameters.FirstOrDefault(p => 
                    p.Name == "Transform" || p.Name == "World" || p.Name == "WorldMatrix");
                
                if (transformParam != null)
                {
                    transformParam.SetValue(renderItem.WorldMatrix);
                }

                // Set vertex declaration
                GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                GraphicsDevice.Indices = meshPart.IndexBuffer;

                // Draw
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    
                    if (meshPart.PrimitiveCount > 0)
                    {
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            meshPart.VertexOffset,
                            meshPart.StartIndex,
                            meshPart.PrimitiveCount,
                            VertexBuffer binding);
                        
                        _drawCalls++;
                        _triangles += meshPart.PrimitiveCount * 3;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        /// <summary>
        /// Disposes the renderer resources.
        /// </summary>
        public void Dispose()
        {
            _instancesBuffer?.Dispose();
            _instancesBuffer = null;
            _initialized = false;
            _log.LogInformation("ForwardRenderer disposed");
        }

        /// <summary>
        /// Render item for batching.
        /// </summary>
        private class RenderItem
        {
            public ModelMeshPart MeshPart { get; set; } = null!;
            public Matrix WorldMatrix { get; set; }
            public AnimationsController? Controller { get; set; }
        }
    }
}
