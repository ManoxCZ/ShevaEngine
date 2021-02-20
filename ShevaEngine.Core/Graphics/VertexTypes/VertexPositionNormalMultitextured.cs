using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace ShevaEngine.Core
{
    /// <summary>
	/// vertex for multitexturing
	/// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexPositionNormalMultitextured : IVertexType
    {
        [FieldOffset(0)] public Vector3 Position;
        [FieldOffset(12)] public Vector3 Normal;
        [FieldOffset(24)] public Vector4 TextureCoordinate;
        [FieldOffset(40)] public Vector4 TexWeights;
        [FieldOffset(56)] public Vector4 TexWeights2;

        public VertexDeclaration VertexDeclaration => new VertexDeclaration(new[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(4 * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(4 * 6, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(4 * 10, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(4 * 14, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2)
        });
    }
}
