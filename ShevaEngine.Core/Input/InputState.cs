using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Input state.
    /// </summary>
    public struct InputState
    {
        public GameTime Time { get; private set; }
        public MouseState MouseState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public InputState(GameTime time)
        {
            Time = time;
            MouseState = Mouse.GetState();
            KeyboardState = Keyboard.GetState();
        }
    }
}
