using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core.Graphics.Materials
{
    public class PhysicallyBasedMaterial : TexturedNormalMaterial
    {
        private readonly EffectParameter _metallicTexture;
        private readonly EffectParameter _roughnessTexture;
        private readonly EffectParameter _aoTexture;
        private readonly EffectParameter _enviroTexture;
        private readonly EffectParameter _irradianceTexture;
        private readonly EffectParameter _brdfLutTexture;

        private readonly EffectParameter _uniformRoughness;
        private readonly EffectParameter _uniformMetallic;

        private readonly EffectParameter _useAOTexture;
        private readonly EffectParameter _useMetallicTexture;
        private readonly EffectParameter _useRoughnessTexture;

        protected PhysicallyBasedMaterial(Effect effect)
            : base(effect)
        {
            _metallicTexture = GetParameter("MetallicTexture");
            _roughnessTexture = GetParameter("RoughnessTexture");
            _aoTexture = GetParameter("AOTexture");
            _enviroTexture = GetParameter("EnviroTexture");
            _irradianceTexture = GetParameter("IrradianceTexture");
            _brdfLutTexture = GetParameter("BrdfLutTexture");

            _useAOTexture = GetParameter("_useAOTexture");
            _useMetallicTexture = GetParameter("_useMetallicTexture");
            _useRoughnessTexture = GetParameter("_useRoughnessTexture");

            _uniformRoughness = GetParameter("_uniformRoughness");
            _uniformMetallic = GetParameter("_uniformMetallic");
        }

        public PhysicallyBasedMaterial()
            : this(ShevaGame.Instance.Content.Load<Effect>(@"Content\Effects\PBR"))
        {
        }

        public bool UseAo
        {
            get => _useAOTexture?.GetValueBoolean() == true;
            set => _useAOTexture?.SetValue(value);
        }

        public bool UseRoughnessTexture
        {
            get => _useRoughnessTexture?.GetValueBoolean() == true;
            set => _useRoughnessTexture?.SetValue(value);
        }

        public bool UseMetallicTexture
        {
            get => _useMetallicTexture?.GetValueBoolean() == true;
            set => _useMetallicTexture?.SetValue(value);
        }

        public float UniformRoughness
        {
            get => (float)_uniformRoughness?.GetValueSingle();
            set => _uniformRoughness?.SetValue(value);
        }

        public float UniformMetallic
        {
            get => (float)_uniformMetallic?.GetValueSingle();
            set => _uniformMetallic?.SetValue(value);
        }

        public Texture2D MetallicTexture
        {
            get => _metallicTexture?.GetValueTexture2D();
            set => _metallicTexture?.SetValue(value);
        }

        public Texture2D RoughnessTexture
        {
            get => _roughnessTexture?.GetValueTexture2D();
            set => _roughnessTexture?.SetValue(value);
        }

        public Texture2D AOTexture
        {
            get => _aoTexture?.GetValueTexture2D();
            set => _aoTexture?.SetValue(value);
        }

        public TextureCube EnviroTexture
        {
            get => _enviroTexture?.GetValueTextureCube();
            set => _enviroTexture?.SetValue(value);
        }

        public TextureCube IrradianceTexture
        {
            get => _irradianceTexture?.GetValueTextureCube();
            set => _irradianceTexture?.SetValue(value);
        }

        public Texture2D BrdfLutTexture
        {
            get => _brdfLutTexture?.GetValueTexture2D();
            set => _brdfLutTexture?.SetValue(value);
        }
    }
}