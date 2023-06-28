using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ShevaEngine.Core.UI;

public interface ILayer
{
    object DataContext { get; set; }

    bool IsActive { get; set; }

    void OnWindowResize(int width, int height);

    void Update(GameTime time);

    bool UpdateInput(in InputState state);

    bool UpdateKeyUpEvent(Keys key);

    bool UpdateKeyDownEvent(Keys key);

    bool UpdateInputTextEvent(char key);

    void Draw(GameTime time);    
}
