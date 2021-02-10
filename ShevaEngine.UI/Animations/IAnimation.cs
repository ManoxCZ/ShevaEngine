using Microsoft.Xna.Framework;

namespace ShevaEngine.UI
{
	/// <summary>
	/// IAnimation.
	/// </summary>
	public interface IAnimation
	{
		void Start(GameTime time);
		void Update(GameTime time);
	}
}
