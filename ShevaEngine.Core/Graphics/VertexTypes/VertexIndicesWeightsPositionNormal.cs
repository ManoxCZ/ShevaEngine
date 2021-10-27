using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace ShevaEngine.Core
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexIndicesWeightsPositionNormal : IVertexType
    {
        [FieldOffset(0)] public byte BlendIndex0;
        [FieldOffset(1)] public byte BlendIndex1;
        [FieldOffset(2)] public byte BlendIndex2;
        [FieldOffset(3)] public byte BlendIndex3;
        [FieldOffset(4)] public Vector4 BlendWeights;
        [FieldOffset(20)] public Vector3 Position;
        [FieldOffset(32)] public Vector3 Normal;

        public VertexDeclaration VertexDeclaration => new VertexDeclaration(new[]
        {
            new VertexElement( 0, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
            new VertexElement( 4, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
        });


        public VertexIndicesWeightsPositionNormal(Vector3 position, Vector3 normal, Vector4 blendWeights, byte blendIndex0, byte blendIndex1, byte blendIndex2, byte blendIndex3)
        {
            BlendIndex0 = blendIndex0;
            BlendIndex1 = blendIndex1;
            BlendIndex2 = blendIndex2;
            BlendIndex3 = blendIndex3;
            BlendWeights = blendWeights;
            Position = position;
            Normal = normal;
        }
    }
}
