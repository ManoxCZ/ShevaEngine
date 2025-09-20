using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ShevaEngine.Core;

public struct InputState
{
    public GameTime Time { get; private set; }
    public MouseState MouseState { get; private set; }
    public KeyboardState KeyboardState { get; private set; }

    public InputState(GameTime time)
    {
        Time = time;
        MouseState = Mouse.GetState();
        KeyboardState = Keyboard.GetState();
    }
}
