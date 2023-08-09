using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.NoesisUI;

internal class NoesisRenderTarget : Noesis.RenderTarget
{
    public string Label { get; }
    public RenderTarget2D RenderTarget2D { get; }


    public NoesisRenderTarget(string label, RenderTarget2D renderTarget2D)
    {
        Label = label;
        RenderTarget2D = renderTarget2D;
    }

    public override Noesis.Texture Texture => new NoesisTexture(Label + "_Texture", RenderTarget2D);
}
