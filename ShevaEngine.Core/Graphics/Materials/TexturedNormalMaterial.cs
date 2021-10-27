using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core
{

    public class TexturedNormalMaterial : TexturedMaterial
    {
        private EffectParameter _normalTextureParameter;
        public Texture2D NormalTexture
        {
            get => _normalTextureParameter?.GetValueTexture2D();
            set => _normalTextureParameter?.SetValue(value);
        }


        /// <summary>
        /// Textured material.
        /// </summary>
        protected TexturedNormalMaterial(Effect effect)
            : base(effect)
        {
            _normalTextureParameter = GetParameter("NormalTexture");
        }

        /// <summary>
        /// Textured material.
        /// </summary>
        public TexturedNormalMaterial()
            : this(ShevaGame.Instance.Content.Load<Effect>(@"Content\Effects\TexturedNormal"))
        {

        }
    }
}
