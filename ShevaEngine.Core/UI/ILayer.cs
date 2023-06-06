using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace ShevaEngine.Core.UI;

public interface ILayer
{
    object DataContext { get; set; }

    bool IsActive { get; set; }

    void OnWindowResize(int width, int height);

    void Update(GameTime time);

    public bool UpdateInput(InputState state);

    public void Draw(GameTime time);

    Task<IViewport> GetViewport(string name);
}
