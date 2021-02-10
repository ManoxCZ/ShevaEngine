using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Quad mesh.
	/// </summary>
	public class QuadMesh
	{
		public static Model Create(GraphicsDevice device, Axis upAxe, Material material = null)
		{
			float x = 1;
			float y = 1;
			float z = 1;
			float yComp = 1;

			Vector3 normal = Vector3.Zero;

			switch (upAxe)
			{
				case Axis.X:
					x = 0;
					normal = Vector3.UnitX;
					break;
				case Axis.Y:
					y = 0;
					normal = Vector3.UnitY;
					break;
				case Axis.Z:
					z = 0;
					yComp = -1;
					normal = Vector3.UnitZ;
					break;				
			}
			

			VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[]
			{
				new VertexPositionNormalTexture()
				{
					Position = new Vector3(-x, -y, -z),
					Normal = normal,
					TextureCoordinate = new Vector2(0, 0)
				},

				new VertexPositionNormalTexture()
				{
					Position = new Vector3(-x, yComp * y, z),
					Normal = normal,
					TextureCoordinate = new Vector2(0, 1)
				},

				new VertexPositionNormalTexture()
				{
					Position = new Vector3(x, y, z),
					Normal = normal,
					TextureCoordinate = new Vector2(1, 1)
				},

				new VertexPositionNormalTexture()
				{
					Position = new Vector3(x, yComp * y, -z),
					Normal = normal,
					TextureCoordinate = new Vector2(1, 0)
				},
			};

			ushort[] indices = new ushort[]
			{
				0, 1, 2,
				0, 2, 3
			};

			return ModelExtensions.CreateModel(vertices, indices, material);
		}
	}
}
