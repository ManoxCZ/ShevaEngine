﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace ShevaEngine.Core
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexPositionNormalTangent : IVertexType
    {
        [FieldOffset(0)] public Vector3 Position;
        [FieldOffset(12)] public Vector3 Normal;
        [FieldOffset(24)] public Vector3 Tangent;

        public VertexDeclaration VertexDeclaration => new VertexDeclaration(new[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(4 * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(4 * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(4 * 8, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
        });

        public VertexPositionNormalTangent(Vector3 position, Vector3 normal, Vector3 tangent)
        {
            Position = position;
            Normal = normal;            
            Tangent = tangent;
        }
    }
}
