using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;

namespace ShevaEngine.Terrain
{
    public class TerrainMaterial : MaterialWithLights
    {
        private EffectParameter _heightmapTextureParameter;
        private EffectParameter _splatTextureParameter;
        private EffectParameter _textureChannel0Parameter;
        private EffectParameter _textureChannel1Parameter;
        private EffectParameter _textureChannel2Parameter;
        private EffectParameter _textureChannel3Parameter;
        private EffectParameter _textureChannel0NormParameter;
        private EffectParameter _textureChannel1NormParameter;
        private EffectParameter _textureChannel2NormParameter;
        private EffectParameter _textureChannel3NormParameter;
        public Texture2D HeightmapTexture
        {
            get => _heightmapTextureParameter?.GetValueTexture2D();
            set => _heightmapTextureParameter?.SetValue(value);
        }
        public Texture2D SplatMapTexture
        {
            get => _splatTextureParameter?.GetValueTexture2D();
            set => _splatTextureParameter?.SetValue(value);
        }
        public Texture2D TextureChannel0
        {
            get => _textureChannel0Parameter?.GetValueTexture2D();
            set => _textureChannel0Parameter?.SetValue(value);
        }
        public Texture2D TextureChannel1
        {
            get => _textureChannel1Parameter?.GetValueTexture2D();
            set => _textureChannel1Parameter?.SetValue(value);
        }
        public Texture2D TextureChannel2
        {
            get => _textureChannel2Parameter?.GetValueTexture2D();
            set => _textureChannel2Parameter?.SetValue(value);
        }
        public Texture2D TextureChannel3
        {
            get => _textureChannel3Parameter?.GetValueTexture2D();
            set => _textureChannel3Parameter?.SetValue(value);
        }
        public Texture2D TextureChannel0Norm
        {
            get => _textureChannel0NormParameter?.GetValueTexture2D();
            set => _textureChannel0NormParameter?.SetValue(value);
        }
        public Texture2D TextureChannel1Norm
        {
            get => _textureChannel1NormParameter?.GetValueTexture2D();
            set => _textureChannel1NormParameter?.SetValue(value);
        }
        public Texture2D TextureChannel2Norm
        {
            get => _textureChannel2NormParameter?.GetValueTexture2D();
            set => _textureChannel2NormParameter?.SetValue(value);
        }
        public Texture2D TextureChannel3Norm
        {
            get => _textureChannel3NormParameter?.GetValueTexture2D();
            set => _textureChannel3NormParameter?.SetValue(value);
        }

        /// <summary>
        /// Colored material.
        /// </summary>
        protected TerrainMaterial(Effect effect)
            : base(effect)
        {
            _heightmapTextureParameter = GetParameter(nameof(HeightmapTexture));
            _splatTextureParameter = GetParameter(nameof(SplatMapTexture));
            _textureChannel0Parameter = GetParameter(nameof(TextureChannel0));
            _textureChannel1Parameter = GetParameter(nameof(TextureChannel1));
            _textureChannel2Parameter = GetParameter(nameof(TextureChannel2));
            _textureChannel3Parameter = GetParameter(nameof(TextureChannel3));
            _textureChannel0NormParameter = GetParameter(nameof(TextureChannel0Norm));
            _textureChannel1NormParameter = GetParameter(nameof(TextureChannel1Norm));
            _textureChannel2NormParameter = GetParameter(nameof(TextureChannel2Norm));
            _textureChannel3NormParameter = GetParameter(nameof(TextureChannel3Norm));

            CastShadows = false;
            ReceiveShadows = true;
        }

        /// <summary>
        /// Colored material.
        /// </summary>
        public TerrainMaterial()
            : this(ShevaGame.Instance.Content.Load<Effect>(@"Content\Effects\Terrain"))
        {
        }
    }
}
