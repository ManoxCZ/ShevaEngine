//using Microsoft.Xna.Framework.Graphics;

//namespace ShevaEngine.Core
//{

//	public class TexturedNormalEmissiveMaterial : TexturedNormalMaterial
//	{
//		private EffectParameter _emissiveTextureParameter;
//		public Texture2D EmissiveTexture
//		{
//			get => _emissiveTextureParameter?.GetValueTexture2D();
//			set => _emissiveTextureParameter?.SetValue(value);
//		}


//		/// <summary>
//		/// Textured emissive material.
//		/// </summary>
//		protected TexturedNormalEmissiveMaterial(Effect effect)
//			: base(effect)
//		{
//			_emissiveTextureParameter = GetParameter("EmissiveTexture");
//		}

//		/// <summary>
//		/// Textured emissive material.
//		/// </summary>
//		public TexturedNormalEmissiveMaterial()
//			: this(ShevaGame.Instance.Content.Load<Effect>(@"Content\Effects\TexturedNormalEmissive"))
//		{

//		}		
//	}
//}
