using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.Graphics;
using System;

namespace ShevaEngine.Renderer.Materials
{
    /// <summary>
    /// Physically Based Rendering (PBR) Material.
    /// Implements a standard PBR workflow with support for:
    /// - Albedo (Base Color)
    /// - Metallic and Roughness
    /// - Normal Mapping
    /// - Ambient Occlusion
    /// - Emissive
    /// - Image-Based Lighting (IBL)
    /// - Shadow mapping
    /// </summary>
    public class PBRMaterial : Material
    {
        private readonly ILogger _log;

        #region Albedo Properties

        private readonly EffectParameter? _albedoColorParameter;
        private readonly EffectParameter? _albedoTextureParameter;
        private readonly EffectParameter? _useAlbedoTextureParameter;

        /// <summary>
        /// Gets or sets the albedo (base) color.
        /// </summary>
        public Color AlbedoColor
        {
            get => Color.FromNonPremultiplied(_albedoColorParameter?.GetValueVector4() ?? Vector4.One);
            set => _albedoColorParameter?.SetValue(value.ToVector4());
        }

        /// <summary>
        /// Gets or sets the albedo texture.
        /// </summary>
        public Texture2D? AlbedoTexture
        {
            get => _albedoTextureParameter?.GetValueTexture2D();
            set => _albedoTextureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets whether to use the albedo texture.
        /// </summary>
        public bool UseAlbedoTexture
        {
            get => _useAlbedoTextureParameter?.GetValueBoolean() == true;
            set => _useAlbedoTextureParameter?.SetValue(value);
        }

        #endregion

        #region Metallic & Roughness Properties

        private readonly EffectParameter? _uniformMetallicParameter;
        private readonly EffectParameter? _uniformRoughnessParameter;
        private readonly EffectParameter? _metallicTextureParameter;
        private readonly EffectParameter? _roughnessTextureParameter;
        private readonly EffectParameter? _useMetallicTextureParameter;
        private readonly EffectParameter? _useRoughnessTextureParameter;

        /// <summary>
        /// Gets or sets the uniform metallic value (0-1).
        /// </summary>
        public float UniformMetallic
        {
            get => _uniformMetallicParameter?.GetValueSingle() ?? 0.0f;
            set => _uniformMetallicParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the uniform roughness value (0-1).
        /// </summary>
        public float UniformRoughness
        {
            get => _uniformRoughnessParameter?.GetValueSingle() ?? 0.5f;
            set => _uniformRoughnessParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the metallic texture.
        /// </summary>
        public Texture2D? MetallicTexture
        {
            get => _metallicTextureParameter?.GetValueTexture2D();
            set => _metallicTextureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the roughness texture.
        /// </summary>
        public Texture2D? RoughnessTexture
        {
            get => _roughnessTextureParameter?.GetValueTexture2D();
            set => _roughnessTextureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets whether to use the metallic texture.
        /// </summary>
        public bool UseMetallicTexture
        {
            get => _useMetallicTextureParameter?.GetValueBoolean() == true;
            set => _useMetallicTextureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets whether to use the roughness texture.
        /// </summary>
        public bool UseRoughnessTexture
        {
            get => _useRoughnessTextureParameter?.GetValueBoolean() == true;
            set => _useRoughnessTextureParameter?.SetValue(value);
        }

        #endregion

        #region Normal Mapping Properties

        private readonly EffectParameter? _normalTextureParameter;
        private readonly EffectParameter? _normalSamplerParameter;
        private readonly EffectParameter? _normalScaleParameter;
        private readonly EffectParameter? _useNormalTextureParameter;

        /// <summary>
        /// Gets or sets the normal texture.
        /// </summary>
        public Texture2D? NormalTexture
        {
            get => _normalTextureParameter?.GetValueTexture2D();
            set => _normalTextureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the normal scale (intensity).
        /// </summary>
        public float NormalScale
        {
            get => _normalScaleParameter?.GetValueSingle() ?? 1.0f;
            set => _normalScaleParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets whether to use normal mapping.
        /// </summary>
        public bool UseNormalTexture
        {
            get => _useNormalTextureParameter?.GetValueBoolean() == true;
            set => _useNormalTextureParameter?.SetValue(value);
        }

        #endregion

        #region Ambient Occlusion Properties

        private readonly EffectParameter? _aoTextureParameter;
        private readonly EffectParameter? _aoValueParameter;
        private readonly EffectParameter? _useAOTextureParameter;

        /// <summary>
        /// Gets or sets the ambient occlusion texture.
        /// </summary>
        public Texture2D? AOTexture
        {
            get => _aoTextureParameter?.GetValueTexture2D();
            set => _aoTextureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the uniform AO value (0-1).
        /// </summary>
        public float AOValue
        {
            get => _aoValueParameter?.GetValueSingle() ?? 1.0f;
            set => _aoValueParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets whether to use the AO texture.
        /// </summary>
        public bool UseAOTexture
        {
            get => _useAOTextureParameter?.GetValueBoolean() == true;
            set => _useAOTextureParameter?.SetValue(value);
        }

        #endregion

        #region Emissive Properties

        private readonly EffectParameter? _emissiveColorParameter;
        private readonly EffectParameter? _emissiveIntensityParameter;
        private readonly EffectParameter? _emissiveTextureParameter;
        private readonly EffectParameter? _useEmissiveTextureParameter;

        /// <summary>
        /// Gets or sets the emissive color.
        /// </summary>
        public Color EmissiveColor
        {
            get => Color.FromNonPremultiplied(_emissiveColorParameter?.GetValueVector3() ?? Vector3.Zero);
            set => _emissiveColorParameter?.SetValue(value.ToVector3());
        }

        /// <summary>
        /// Gets or sets the emissive intensity.
        /// </summary>
        public float EmissiveIntensity
        {
            get => _emissiveIntensityParameter?.GetValueSingle() ?? 1.0f;
            set => _emissiveIntensityParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the emissive texture.
        /// </summary>
        public Texture2D? EmissiveTexture
        {
            get => _emissiveTextureParameter?.GetValueTexture2D();
            set => _emissiveTextureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets whether to use the emissive texture.
        /// </summary>
        public bool UseEmissiveTexture
        {
            get => _useEmissiveTextureParameter?.GetValueBoolean() == true;
            set => _useEmissiveTextureParameter?.SetValue(value);
        }

        #endregion

        #region Height/Parallax Properties

        private readonly EffectParameter? _heightTextureParameter;
        private readonly EffectParameter? _heightScaleParameter;
        private readonly EffectParameter? _useHeightTextureParameter;

        /// <summary>
        /// Gets or sets the height texture.
        /// </summary>
        public Texture2D? HeightTexture
        {
            get => _heightTextureParameter?.GetValueTexture2D();
            set => _heightTextureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the height scale for parallax mapping.
        /// </summary>
        public float HeightScale
        {
            get => _heightScaleParameter?.GetValueSingle() ?? 0.05f;
            set => _heightScaleParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets whether to use height mapping.
        /// </summary>
        public bool UseHeightTexture
        {
            get => _useHeightTextureParameter?.GetValueBoolean() == true;
            set => _useHeightTextureParameter?.SetValue(value);
        }

        #endregion

        #region IBL Properties

        private readonly EffectParameter? _enviroTextureParameter;
        private readonly EffectParameter? _irradianceTextureParameter;
        private readonly EffectParameter? _brdfLutTextureParameter;
        private readonly EffectParameter? _envRotationParameter;

        /// <summary>
        /// Gets or sets the environment texture for reflections.
        /// </summary>
        public TextureCube? EnviroTexture
        {
            get => _enviroTextureParameter?.GetValueTextureCube();
            set => _enviroTextureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the irradiance texture for diffuse IBL.
        /// </summary>
        public TextureCube? IrradianceTexture
        {
            get => _irradianceTextureParameter?.GetValueTextureCube();
            set => _irradianceTextureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the BRDF lookup texture.
        /// </summary>
        public Texture2D? BrdfLutTexture
        {
            get => _brdfLutTextureParameter?.GetValueTexture2D();
            set => _brdfLutTextureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the environment rotation (Euler angles).
        /// </summary>
        public Vector3 EnvRotation
        {
            get => _envRotationParameter?.GetValueVector3() ?? Vector3.Zero;
            set => _envRotationParameter?.SetValue(value);
        }

        #endregion

        #region Shadow Properties

        private readonly EffectParameter? _light1ShadowMapParameter;
        private readonly EffectParameter? _depthBiasParameter;

        /// <summary>
        /// Gets or sets the primary shadow map.
        /// </summary>
        public Texture2D? Light1ShadowMap
        {
            get => _light1ShadowMapParameter?.GetValueTexture2D();
            set => _light1ShadowMapParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the shadow depth bias.
        /// </summary>
        public float DepthBias
        {
            get => _depthBiasParameter?.GetValueSingle() ?? 0.05f;
            set => _depthBiasParameter?.SetValue(value);
        }

        #endregion

        #region Screen & Post-Processing Properties

        private readonly EffectParameter? _screenResolutionParameter;
        private readonly EffectParameter? _exposureParameter;
        private readonly EffectParameter? _gammaParameter;

        /// <summary>
        /// Gets or sets the screen resolution.
        /// </summary>
        public Vector2 ScreenResolution
        {
            get => _screenResolutionParameter?.GetValueVector2() ?? Vector2.Zero;
            set => _screenResolutionParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the exposure value for HDR.
        /// </summary>
        public float Exposure
        {
            get => _exposureParameter?.GetValueSingle() ?? 1.0f;
            set => _exposureParameter?.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the gamma value for gamma correction.
        /// </summary>
        public float Gamma
        {
            get => _gammaParameter?.GetValueSingle() ?? 2.2f;
            set => _gammaParameter?.SetValue(value);
        }

        #endregion

        #region Constructor & Initialization

        /// <summary>
        /// Creates a new PBR material with the specified effect.
        /// </summary>
        public PBRMaterial(Effect effect) : base(effect)
        {
            _log = ShevaServices.GetService<ILoggerFactory>().CreateLogger(GetType());

            // Initialize all parameter references
            InitializeParameters();
        }

        /// <summary>
        /// Creates a new PBR material by loading the default PBR effect.
        /// </summary>
        public PBRMaterial() : this(LoadDefaultEffect())
        {
        }

        /// <summary>
        /// Loads the default PBR effect.
        /// </summary>
        private static Effect LoadDefaultEffect()
        {
            try
            {
                return ShevaGame.Instance.Content.Load<Effect>(@"Content\Effects\PBR");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to load default PBR effect. Make sure the effect is compiled and available in the content pipeline.",
                    ex);
            }
        }

        /// <summary>
        /// Initializes all effect parameter references.
        /// </summary>
        private void InitializeParameters()
        {
            // Albedo
            _albedoColorParameter = GetParameter("AlbedoColor");
            _albedoTextureParameter = GetParameter("AlbedoTexture");
            _useAlbedoTextureParameter = GetParameter("UseAlbedoTexture");

            // Metallic & Roughness
            _uniformMetallicParameter = GetParameter("UniformMetallic");
            _uniformRoughnessParameter = GetParameter("UniformRoughness");
            _metallicTextureParameter = GetParameter("MetallicTexture");
            _roughnessTextureParameter = GetParameter("RoughnessTexture");
            _useMetallicTextureParameter = GetParameter("UseMetallicTexture");
            _useRoughnessTextureParameter = GetParameter("UseRoughnessTexture");

            // Normal
            _normalTextureParameter = GetParameter("NormalTexture");
            _normalSamplerParameter = GetParameter("NormalSampler");
            _normalScaleParameter = GetParameter("NormalScale");
            _useNormalTextureParameter = GetParameter("UseNormalTexture");

            // AO
            _aoTextureParameter = GetParameter("AOTexture");
            _aoValueParameter = GetParameter("AOValue");
            _useAOTextureParameter = GetParameter("UseAOTexture");

            // Emissive
            _emissiveColorParameter = GetParameter("EmissiveColor");
            _emissiveIntensityParameter = GetParameter("EmissiveIntensity");
            _emissiveTextureParameter = GetParameter("EmissiveTexture");
            _useEmissiveTextureParameter = GetParameter("UseEmissiveTexture");

            // Height
            _heightTextureParameter = GetParameter("HeightTexture");
            _heightScaleParameter = GetParameter("HeightScale");
            _useHeightTextureParameter = GetParameter("UseHeightTexture");

            // IBL
            _enviroTextureParameter = GetParameter("EnviroTexture");
            _irradianceTextureParameter = GetParameter("IrradianceTexture");
            _brdfLutTextureParameter = GetParameter("BrdfLutTexture");
            _envRotationParameter = GetParameter("EnvRotation");

            // Shadows
            _light1ShadowMapParameter = GetParameter("Light1ShadowMap");
            _depthBiasParameter = GetParameter("DepthBias");

            // Screen & Post-Processing
            _screenResolutionParameter = GetParameter("ScreenResolution");
            _exposureParameter = GetParameter("Exposure");
            _gammaParameter = GetParameter("Gamma");

            // Set default values
            SetDefaultValues();
        }

        /// <summary>
        /// Sets default parameter values.
        /// </summary>
        private void SetDefaultValues()
        {
            // Material defaults
            AlbedoColor = Color.White;
            UniformMetallic = 0.0f;
            UniformRoughness = 0.5f;
            NormalScale = 1.0f;
            AOValue = 1.0f;
            EmissiveColor = Color.Black;
            EmissiveIntensity = 1.0f;
            HeightScale = 0.05f;

            // Toggles
            UseAlbedoTexture = true;
            UseMetallicTexture = false;
            UseRoughnessTexture = false;
            UseNormalTexture = true;
            UseAOTexture = false;
            UseEmissiveTexture = false;
            UseHeightTexture = false;

            // Environment
            EnvRotation = Vector3.Zero;

            // Shadows
            DepthBias = 0.05f;

            // Post-processing
            Exposure = 1.0f;
            Gamma = 2.2f;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets a parameter by name from the effect.
        /// </summary>
        private EffectParameter? GetParameter(string name)
        {
            EffectParameter? parameter = Parameters.FirstOrDefault(p => p.Name == name);
            if (parameter == null)
            {
                _log.LogWarning($"Effect parameter '{name}' not found");
            }
            return parameter;
        }

        /// <summary>
        /// Sets all material textures at once.
        /// </summary>
        public void SetTextures(
            Texture2D? albedo = null,
            Texture2D? metallic = null,
            Texture2D? roughness = null,
            Texture2D? normal = null,
            Texture2D? ao = null,
            Texture2D? emissive = null,
            Texture2D? height = null)
        {
            AlbedoTexture = albedo;
            MetallicTexture = metallic;
            RoughnessTexture = roughness;
            NormalTexture = normal;
            AOTexture = ao;
            EmissiveTexture = emissive;
            HeightTexture = height;
        }

        /// <summary>
        /// Sets all IBL textures at once.
        /// </summary>
        public void SetIBLTextures(
            TextureCube? enviro = null,
            TextureCube? irradiance = null,
            Texture2D? brdfLut = null)
        {
            EnviroTexture = enviro;
            IrradianceTexture = irradiance;
            BrdfLutTexture = brdfLut;
        }

        /// <summary>
        /// Configures the material for a specific workflow.
        /// </summary>
        public void ConfigureForWorkflows(
            bool useAlbedoTex = true,
            bool useMetallicTex = false,
            bool useRoughnessTex = false,
            bool useNormalTex = true,
            bool useAOTex = false,
            bool useEmissiveTex = false)
        {
            UseAlbedoTexture = useAlbedoTex;
            UseMetallicTexture = useMetallicTex;
            UseRoughnessTexture = useRoughnessTex;
            UseNormalTexture = useNormalTex;
            UseAOTexture = useAOTex;
            UseEmissiveTexture = useEmissiveTex;
        }

        /// <summary>
        /// Sets metallic and roughness values.
        /// </summary>
        public void SetMetallicRoughness(float metallic, float roughness)
        {
            UniformMetallic = metallic;
            UniformRoughness = roughness;
        }

        #endregion
    }
}
