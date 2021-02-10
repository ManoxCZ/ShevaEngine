using Microsoft.Xna.Framework;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Shadow.
	/// </summary>
	public abstract class Shadow
	{

		/// <summary>
		/// Get light viewProj matrix.
		/// </summary>		
		public abstract Matrix GetLightViewProjMatrix();		

		/// <summary>
		/// Update method.
		/// </summary>
		public abstract void Update(GameTime gametime, Scene scene, Light light, Camera camera);
	}
}
