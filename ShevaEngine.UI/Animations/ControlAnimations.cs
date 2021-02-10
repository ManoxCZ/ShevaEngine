using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Control animations.
	/// </summary>
	public class ControlAnimations : List<IAnimation>
	{
		/// <summary>
		/// Start method.
		/// </summary>		
		public void Start(GameTime time)
		{
			foreach (IAnimation animation in this)
				animation.Start(time);
		}

		/// <summary>
		/// Update method.
		/// </summary>		
		public void Update(GameTime time)
		{
			foreach (IAnimation animation in this)
				animation.Update(time);			
		}
	}
}
