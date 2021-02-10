using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ShevaEngine.Core
{
	public class PlaneMesh
	{
		/// <summary>
		/// Generate.
		/// </summary>		
		public static Model GenerateModel(int columnsCount, int rowsCount)
		{
			ModelMesh modelMesh = GenerateModelMesh(columnsCount, rowsCount);

			return new Model(ShevaGame.Instance.GraphicsDevice, new List<ModelBone>(), new List<ModelMesh> { modelMesh });			
		}

		/// <summary>
		/// Generate model mesh.
		/// </summary>		
		public static ModelMesh GenerateModelMesh(int columnsCount, int rowsCount)
		{
			GraphicsDevice device = ShevaGame.Instance.GraphicsDevice;

			List<VertexPositionTexture> vertices = new List<VertexPositionTexture>();
			
			for (int iRow = 0; iRow < rowsCount; iRow++)
			{
				for (int iCol = 0; iCol < columnsCount; iCol++)
				{
					vertices.Add(new VertexPositionTexture()
					{
						Position = new Vector3(
							(iCol / (float)(columnsCount - 1) - 0.5f) * 2.5f,
							(iRow / (float)(rowsCount - 1) - 0.5f) * 2.5f,
							0),
						TextureCoordinate = new Vector2(
							(iCol / (float)(columnsCount - 1)),
							1.0f - (iRow / (float)(rowsCount - 1)))
					});
				}
			}

			VertexBuffer vertexBuffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
			vertexBuffer.SetData(vertices.ToArray());
			MaterialsManager.UpdateVertexDeclarationTag(vertexBuffer.VertexDeclaration);

			List<ushort> indices = new List<ushort>();

			for (int iRow = 0; iRow < rowsCount - 1; iRow++)
			{
				for (int iCol = 0; iCol < columnsCount - 1; iCol++)
				{
					ushort index0 = (ushort)(iCol + iRow * columnsCount);
					ushort index1 = (ushort)(iCol + (iRow + 1) * columnsCount);
					ushort index2 = (ushort)((iCol + 1) + (iRow + 1) * columnsCount);
					ushort index3 = (ushort)((iCol + 1) + iRow * columnsCount);

					indices.Add(index0);
					indices.Add(index1);
					indices.Add(index2);

					indices.Add(index0);
					indices.Add(index2);
					indices.Add(index3);
				}
			}

			IndexBuffer indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Count, BufferUsage.WriteOnly);
			indexBuffer.SetData(indices.ToArray());

			return new ModelMesh(ShevaGame.Instance.GraphicsDevice, new List<ModelMeshPart>
			{
				new ModelMeshPart()
				{
					IndexBuffer = indexBuffer,
					VertexBuffer = vertexBuffer,
					NumVertices = vertices.Count,
					PrimitiveCount = indices.Count / 3,
					StartIndex = 0,
					VertexOffset = 0
				}
			});			
		}
	}
}
