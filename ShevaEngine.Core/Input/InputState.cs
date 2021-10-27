using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Input state.
    /// </summary>
    public class InputState
    {
        public GameTime Time { get; private set; }
        public MouseState MouseState { get; private set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public InputState(GameTime time, GameWindow window)
        {
            Time = time;
            MouseState = Mouse.GetState();
        }
    }
}
