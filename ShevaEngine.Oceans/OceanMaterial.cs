using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System;

namespace ShevaEngine.Oceans
{
	/// <summary>
	/// Ocean material.
	/// </summary>
	public class OceanMaterial : MaterialWithLights
	{
		private const int MAX_WAVES = 12; //same in shader
		private EffectParameter _inverseViewProjParameter;
		private EffectParameter _gerstnerWavesCountParameter;		
		private EffectParameter _gerstnerWavesPinch;
		private EffectParameter _gerstnerWaveKWDParameter;
		private EffectParameter _gerstnerWavePAParameter;
		private EffectParameter _normalTextureParameter;
		private EffectParameter _refractionTextureParameter;
		private EffectParameter _depthTextureParameter;
		private EffectParameter _oceanColorParameter;
		private EffectParameter _skyColorParameter;
		private EffectParameter _depthFactorParameter;
		private EffectParameter _lightFactorParameter;

		public Texture2D NormalTexture
		{
			get => _normalTextureParameter?.GetValueTexture2D();
			set => _normalTextureParameter?.SetValue(value);
		}

		public Texture2D RefractionTexture
		{
			get => _refractionTextureParameter?.GetValueTexture2D();
			set => _refractionTextureParameter?.SetValue(value);
		}

		public Texture2D DepthTexture
		{
			get => _depthTextureParameter?.GetValueTexture2D();
			set => _depthTextureParameter?.SetValue(value);
		}
		private readonly Ocean _ocean;

		public Color OceanColor
		{
			get => Color.FromNonPremultiplied(_oceanColorParameter.GetValueVector4());
			set => _oceanColorParameter?.SetValue(value.ToVector4());
		}

		public Color SkyColor
		{
			get => Color.FromNonPremultiplied(_skyColorParameter.GetValueVector4());
			set => _skyColorParameter?.SetValue(value.ToVector4());
		}

		public float DepthFactor
		{
			get => _depthFactorParameter.GetValueSingle();
			set => _depthFactorParameter?.SetValue(value);
		}

		public float LightFactor
		{
			get => _lightFactorParameter.GetValueSingle();
			set => _lightFactorParameter?.SetValue(value);
		}        


        /// <summary>
        /// Ocean material.
        /// </summary>
        protected OceanMaterial(Effect effect, Ocean ocean)
			: base(effect)
		{
			_ocean = ocean;

			_inverseViewProjParameter = GetParameter("InverseViewProjMatrix");
			_gerstnerWavesCountParameter = GetParameter("GerstnerWavesCount");
			
			_gerstnerWavesPinch = GetParameter("Pinch");
			_gerstnerWaveKWDParameter = GetParameter("Waves_K_W_Dir");
			_gerstnerWavePAParameter = GetParameter("Waves_Phase_Amp");

			_normalTextureParameter = GetParameter("NormalTexture");
			_refractionTextureParameter = GetParameter("RefractionTexture");
			_depthTextureParameter = GetParameter("DepthTexture");

			_oceanColorParameter = GetParameter("OceanColor");
			_skyColorParameter = GetParameter("SkyColor");
			_depthFactorParameter = GetParameter("DepthFactor");
			_lightFactorParameter = GetParameter("LightFactor");
			
			UpdateGerstnerWaves(1.0f);

            OceanColor = Color.FromNonPremultiplied(new Vector4(0.4f, 0.35f, 0.06f, 1));
            SkyColor = Color.FromNonPremultiplied(new Vector4(0.6f, 0.47f, 0.26f, 1));
            //OceanColor = Color.FromNonPremultiplied(new Vector4(0.13f, 0.44f, 0.47f, 1));	
            //SkyColor = Color.FromNonPremultiplied(new Vector4(0.8490f, 0.9019f, 1.0f, 1));
            DepthFactor = 0.1f;
			LightFactor = 1.0f;
		}

		
		/// <summary>
		/// Ocean material.
		/// </summary>
		public OceanMaterial(Ocean ocean)
			: this(ShevaGame.Instance.Content.Load<Effect>(@"Content\Effects\Ocean"), ocean)
		{
			
		}

		/// <summary>
		/// Apply.
		/// </summary>
		public override void Apply(MaterialProfile matProfile, Camera camera, float gameTime, VertexDeclaration declaration)
		{
			_inverseViewProjParameter?.SetValue(Matrix.Invert(camera.ViewMatrix * camera.ProjectionMatrix));

			base.Apply(matProfile, camera, gameTime, declaration);
		}

		/// <summary>
		/// Update Gerstner waves.
		/// </summary>
		public void UpdateGerstnerWaves(float pinch = 1.0f)
		{
			_gerstnerWavesCountParameter?.SetValue(Math.Min(_ocean.Waves.Count, MAX_WAVES));

			var kwd = new Vector4[MAX_WAVES];
			var pa = new Vector4[MAX_WAVES];

			for (int i = 0; i < Math.Min(_ocean.Waves.Count, MAX_WAVES); ++i)
			{
				kwd[i] = new Vector4(_ocean.Waves[i].K, _ocean.Waves[i].W, _ocean.Waves[i].Direction.X, _ocean.Waves[i].Direction.Y);
				pa[i] = new Vector4(_ocean.Waves[i].Phase, _ocean.Waves[i].Amplitude, 0.0f, 0.0f);
			}

			_gerstnerWaveKWDParameter?.SetValue(kwd);
			_gerstnerWavePAParameter?.SetValue(pa);
			_gerstnerWavesPinch?.SetValue(pinch);
		}
	}
}
