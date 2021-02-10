using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Input state.
	/// </summary>
	public class InputState
	{		
		public GameTime Time { get; private set; }
		public KeyboardState KeyboardState { get; private set; }
		public MouseState MouseState { get; private set; }
		public GamePadState GamePadState { get; private set; }
		public TouchPanelState TouchPanelState { get; private set; }		

		
		/// <summary>
		/// Constructor.
		/// </summary>
		public InputState(GameTime time, GameWindow window)
		{
			Time = time;
			KeyboardState = Keyboard.GetState();
			MouseState = Mouse.GetState();
			GamePadState = GamePad.GetState(PlayerIndex.One);
			TouchPanelState = TouchPanel.GetState(window);
		}
	}
}
