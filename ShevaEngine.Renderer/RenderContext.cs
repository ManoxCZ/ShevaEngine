using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.Graphics;

namespace ShevaEngine.Renderer
{
    /// <summary>
    /// Represents the rendering context containing all state needed for rendering.
    /// </summary>
    public class RenderContext
    {
        /// <summary>
        /// Gets or sets the graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; set; } = null!;

        /// <summary>
        /// Gets or sets the current camera.
        /// </summary>
        public Camera Camera { get; set; } = null!;

        /// <summary>
        /// Gets or sets the current game time.
        /// </summary>
        public GameTime GameTime { get; set; } = null!;

        /// <summary>
        /// Gets or sets the material profile.
        /// </summary>
        public MaterialProfile Profile { get; set; } = MaterialProfile.Default;

        /// <summary>
        /// Gets the list of lights in the scene.
        /// </summary>
        public LightCollection Lights { get; } = new LightCollection();

        /// <summary>
        /// Gets or sets the environment texture for IBL.
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
        /// Gets or sets the shadow map.
        /// </summary>
        public RenderTarget2D? ShadowMap { get; set; }

        /// <summary>
        /// Gets or sets the exposure value.
        /// </summary>
        public float Exposure { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the gamma value.
        /// </summary>
        public float Gamma { get; set; } = 2.2f;

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
        /// Gets the screen resolution.
        /// </summary>
        public Vector2 ScreenResolution => new Vector2(
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height);

        /// <summary>
        /// Gets or sets the view matrix.
        /// </summary>
        public Matrix ViewMatrix => Camera?.ViewMatrix ?? Matrix.Identity;

        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        public Matrix ProjectionMatrix => Camera?.ProjectionMatrix ?? Matrix.Identity;

        /// <summary>
        /// Creates a new render context.
        /// </summary>
        public RenderContext()
        {
        }

        /// <summary>
        /// Creates a new render context with the specified graphics device and camera.
        /// </summary>
        public RenderContext(GraphicsDevice graphicsDevice, Camera camera)
        {
            GraphicsDevice = graphicsDevice;
            Camera = camera;
        }

        /// <summary>
        /// Adds a light to the scene.
        /// </summary>
        public void AddLight(Light light)
        {
            Lights.Add(light);
        }

        /// <summary>
        /// Clears all lights from the scene.
        /// </summary>
        public void ClearLights()
        {
            Lights.Clear();
        }

        /// <summary>
        /// Sets up the context for rendering.
        /// </summary>
        public void Setup()
        {
            GraphicsDevice.BlendState = BlendState;
            GraphicsDevice.RasterizerState = RasterizerState;
            GraphicsDevice.DepthStencilState = DepthStencilState;
        }

        /// <summary>
        /// Applies the context parameters to an effect.
        /// </summary>
        public void ApplyToEffect(Effect effect)
        {
            EffectParameter? viewParam = effect.Parameters.FirstOrDefault(p => p.Name == "ViewMatrix");
            EffectParameter? projParam = effect.Parameters.FirstOrDefault(p => p.Name == "ProjMatrix");
            EffectParameter? cameraPosParam = effect.Parameters.FirstOrDefault(p => p.Name == "CameraPosition");
            EffectParameter? gameTimeParam = effect.Parameters.FirstOrDefault(p => p.Name == "GameTime");
            EffectParameter? screenResParam = effect.Parameters.FirstOrDefault(p => p.Name == "ScreenResolution");
            EffectParameter? exposureParam = effect.Parameters.FirstOrDefault(p => p.Name == "Exposure");
            EffectParameter? gammaParam = effect.Parameters.FirstOrDefault(p => p.Name == "Gamma");

            viewParam?.SetValue(ViewMatrix);
            projParam?.SetValue(ProjectionMatrix);
            cameraPosParam?.SetValue(Camera?.Position ?? Vector3.Zero);
            gameTimeParam?.SetValue((float)GameTime.TotalGameTime.TotalSeconds);
            screenResParam?.SetValue(ScreenResolution);
            exposureParam?.SetValue(Exposure);
            gammaParam?.SetValue(Gamma);

            // Apply lights
            Lights.ApplyToEffect(effect);

            // Apply IBL textures
            EffectParameter? enviroParam = effect.Parameters.FirstOrDefault(p => p.Name == "EnviroTexture");
            EffectParameter? irradianceParam = effect.Parameters.FirstOrDefault(p => p.Name == "IrradianceTexture");
            EffectParameter? brdfLutParam = effect.Parameters.FirstOrDefault(p => p.Name == "BrdfLutTexture");

            enviroParam?.SetValue(EnvironmentTexture);
            irradianceParam?.SetValue(IrradianceTexture);
            brdfLutParam?.SetValue(BrdfLutTexture);

            // Apply shadow map
            EffectParameter? shadowMapParam = effect.Parameters.FirstOrDefault(p => p.Name == "Light1ShadowMap");
            shadowMapParam?.SetValue(ShadowMap);
        }
    }

    /// <summary>
    /// Collection of lights with helper methods for applying to effects.
    /// </summary>
    public class LightCollection
    {
        private readonly List<Light> _lights = new();

        /// <summary>
        /// Gets the number of lights in the collection.
        /// </summary>
        public int Count => _lights.Count;

        /// <summary>
        /// Gets or sets a light at the specified index.
        /// </summary>
        public Light this[int index] => _lights[index];

        /// <summary>
        /// Adds a light to the collection.
        /// </summary>
        public void Add(Light light)
        {
            _lights.Add(light);
        }

        /// <summary>
        /// Removes a light from the collection.
        /// </summary>
        public bool Remove(Light light)
        {
            return _lights.Remove(light);
        }

        /// <summary>
        /// Clears all lights from the collection.
        /// </summary>
        public void Clear()
        {
            _lights.Clear();
        }

        /// <summary>
        /// Applies the light data to an effect.
        /// </summary>
        public void ApplyToEffect(Effect effect)
        {
            EffectParameter? lightsCountParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightsCount");
            EffectParameter? lightTypesParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightTypes");
            EffectParameter? lightPositionsParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightPositions");
            EffectParameter? lightColorsParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightColors");
            EffectParameter? lightViewProjsParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightViewProjs");
            EffectParameter? lightShadowMapSizesParam = effect.Parameters.FirstOrDefault(p => p.Name == "LightShadowMapSizes");

            int lightCount = Math.Min(_lights.Count, 4);
            lightsCountParam?.SetValue(lightCount);

            if (lightTypesParam != null && lightPositionsParam != null && 
                lightColorsParam != null && lightViewProjsParam != null &&
                lightShadowMapSizesParam != null && lightCount > 0)
            {
                float[] lightTypes = new float[4];
                Vector3[] lightPositions = new Vector3[4];
                Vector4[] lightColors = new Vector4[4];
                Matrix[] lightViewProjs = new Matrix[4];
                Vector2[] shadowMapSizes = new Vector2[4];

                for (int i = 0; i < lightCount; i++)
                {
                    Light light = _lights[i];

                    // Light type: 0=Point, 1=Directional, 2=Spot
                    if (light is DirectionalLight)
                    {
                        lightTypes[i] = 1.0f;
                        lightPositions[i] = light.Direction;
                    }
                    else
                    {
                        lightTypes[i] = 0.0f;
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
        }

        /// <summary>
        /// Returns an enumerator that iterates through the lights.
        /// </summary>
        public IEnumerator<Light> GetEnumerator()
        {
            return _lights.GetEnumerator();
        }
    }
}
