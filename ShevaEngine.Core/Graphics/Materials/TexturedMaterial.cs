using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core
{

    public class TexturedMaterial : ColoredMaterial
    {
        private EffectParameter? _textureParameter;
        public Texture2D? Texture
        {
            get => _textureParameter?.GetValueTexture2D();
            set => _textureParameter?.SetValue(value);
        }


        /// <summary>
        /// Textured material.
        /// </summary>
        protected TexturedMaterial(Effect effect)
            : base(effect)
        {
            _textureParameter = GetParameter("Texture");
        }

        /// <summary>
        /// Textured material.
        /// </summary>
        public TexturedMaterial()
            : this(ShevaGame.Instance.Content.Load<Effect>(@"Content\Effects\Textured"))
        {

        }
    }
}
