using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Vertex matrix.
	/// </summary>
	public struct VertexMatrix : IVertexType
	{
		public VertexDeclaration VertexDeclaration => new VertexDeclaration(new[] {
			new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Color,0),
			new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Color,1),
			new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Color,2),
			new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.Color,3)});

		public Matrix Transform;
	}
}
