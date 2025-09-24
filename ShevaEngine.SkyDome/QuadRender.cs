using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.SkyDomes;

public partial class QuadRenderComponent
{
    private VertexPositionTexture[] _vertices;
    private short[] _indices;

    public QuadRenderComponent()
    {
        _vertices =
            [
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,0)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,0))
                ];

        _indices = [0, 1, 2, 2, 3, 0];
    }

    public void Render(GraphicsDevice graphicsDevice, Vector2 v1, Vector2 v2)
    {
        _vertices[0].Position.X = v2.X;
        _vertices[0].Position.Y = v1.Y;

        _vertices[1].Position.X = v1.X;
        _vertices[1].Position.Y = v1.Y;

        _vertices[2].Position.X = v1.X;
        _vertices[2].Position.Y = v2.Y;

        _vertices[3].Position.X = v2.X;
        _vertices[3].Position.Y = v2.Y;

        graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, 4, _indices, 0, 2);
    }
}
