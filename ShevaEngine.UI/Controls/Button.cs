using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ShevaEngine.Core;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Button class.
	/// </summary>	
	public class Button : Control
	{

		/// <summary>
		/// Constructor.
		/// </summary>
		public Button()
			: base()
		{
			IsSelectAble = true;
		}
	}
}
