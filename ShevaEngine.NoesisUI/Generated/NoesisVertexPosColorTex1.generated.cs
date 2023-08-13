// <auto-generated/>
using System;
﻿using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ShevaEngine.NoesisUI.Generated
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NoesisVertexPosColorTex1 : IVertexType
    {
        private static readonly VertexDeclaration _vertexDeclaration;
        VertexDeclaration IVertexType.VertexDeclaration => _vertexDeclaration;        

		public Vector2 Pos;

        static NoesisVertexPosColorTex1()
		{
			VertexElement[] elements = new VertexElement[] 
            { 
				new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
				new VertexElement(8, VertexElementFormat.Byte4, VertexElementUsage.Color, 0),
				new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),

            };			
			_vertexDeclaration = new VertexDeclaration(elements);
		}
    }
}
