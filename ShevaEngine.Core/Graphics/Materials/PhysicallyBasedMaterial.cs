using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core.Graphics.Materials
{
    public class PhysicallyBasedMaterial : TexturedNormalMaterial
    {
        private EffectParameter _metallicTexture;
        private EffectParameter _roughnessTexture;
        private EffectParameter _aoTexture;

        protected PhysicallyBasedMaterial(Effect effect)
            : base(effect)
        {
            _metallicTexture = GetParameter("MetallicTexture");
            _roughnessTexture = GetParameter("RoughnessTexture");
            _aoTexture = GetParameter("AOTexture");
        }

        public PhysicallyBasedMaterial()
            : this(ShevaGame.Instance.Content.Load<Effect>(@"Content\Effects\PBR"))
        {
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
    }
}