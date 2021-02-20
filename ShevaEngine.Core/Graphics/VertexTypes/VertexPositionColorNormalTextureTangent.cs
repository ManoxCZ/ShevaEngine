using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace ShevaEngine.Core
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexPositionColorNormalTextureTangent : IVertexType
    {
        [FieldOffset(0)] public Vector3 Position;
        [FieldOffset(12)] public Vector3 Normal;
        [FieldOffset(24)] public Vector2 TexCoord;
        [FieldOffset(32)] public Vector3 Tangent;
        [FieldOffset(44)] public Vector4 Color;

        public VertexDeclaration VertexDeclaration => new VertexDeclaration(new[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(4 * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(4 * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(4 * 8, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
            new VertexElement(4 * 11, VertexElementFormat.Vector4, VertexElementUsage.Color, 0)
        });

        public VertexPositionColorNormalTextureTangent(Vector3 position, Vector3 normal, Vector3 tangent, Vector2 texcoord, Color color)
        {
            Position = position;
            Normal = normal;
            TexCoord = texcoord;
            Tangent = tangent;
            Color = color.ToVector4();
        }
    }
}
