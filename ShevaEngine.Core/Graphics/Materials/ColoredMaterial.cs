using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core
{

    public class ColoredMaterial : MaterialWithLights
    {
        private EffectParameter _colorParameter;
        private EffectParameter _specularFactorParameter;
        public Color Color
        {
            get => Color.FromNonPremultiplied(_colorParameter.GetValueVector4());
            set => _colorParameter?.SetValue(value.ToVector4());
        }
        public float SpecularFactor
        {
            get => _specularFactorParameter.GetValueSingle();
            set => _specularFactorParameter?.SetValue(value);
        }

        /// <summary>
        /// Colored material.
        /// </summary>
        protected ColoredMaterial(Effect effect)
            : base(effect)
        {
            _colorParameter = GetParameter("Color");
            _specularFactorParameter = GetParameter("SpecularFactor");
        }

        /// <summary>
        /// Colored material.
        /// </summary>
        public ColoredMaterial()
            : this(ShevaGame.Instance.Content.Load<Effect>(@"Content\Effects\Colored"))
        {

        }
    }
}
