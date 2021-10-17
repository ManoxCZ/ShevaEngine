using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;

namespace ShevaEngine.ParticleSystems
{
    /// <summary>
    /// Particle material.
    /// </summary>
    public class ParticleMaterial : TexturedMaterial
    {
        private EffectParameter _lifetimeParameter;
        public float Lifetime
        {
            get => _lifetimeParameter.GetValueSingle();
            set => _lifetimeParameter.SetValue(value);
        }
        private EffectParameter _startColorParameter;
        public Color StartColor
        {
            get => Color.FromNonPremultiplied(_startColorParameter.GetValueVector4());
            set => _startColorParameter?.SetValue(value.ToVector4());
        }
        private EffectParameter _endColorParameter;
        public Color EndColor
        {
            get => Color.FromNonPremultiplied(_endColorParameter.GetValueVector4());
            set => _endColorParameter?.SetValue(value.ToVector4());
        }
        private EffectParameter _startSizeParameter;
        public float StartSize
        {
            get => _startSizeParameter.GetValueSingle();
            set => _startSizeParameter.SetValue(value);
        }
        private EffectParameter _endSizeParameter;
        public float EndSize
        {
            get => _endSizeParameter.GetValueSingle();
            set => _endSizeParameter.SetValue(value);
        }



        /// <summary>
        /// Particle material.
        /// </summary>
        protected ParticleMaterial(Effect effect)
            : base(effect)
        {
            _lifetimeParameter = GetParameter(nameof(Lifetime));
            _startColorParameter = GetParameter(nameof(StartColor));
            _endColorParameter = GetParameter(nameof(EndColor));
            _startSizeParameter = GetParameter(nameof(StartSize));
            _endSizeParameter = GetParameter(nameof(EndSize));
        }

        /// <summary>
		/// Particle material.
		/// </summary>
		public ParticleMaterial()
            : this(ShevaGame.Instance.Content.Load<Effect>(@"Content\ParticleSystems\Effects\Particle"))
        {
            CastShadows = false;
            ReceiveShadows = false;
            Transparent = true;
            Color = Color.White;
        }
    }
}
