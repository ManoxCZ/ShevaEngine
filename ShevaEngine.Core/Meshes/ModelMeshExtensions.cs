using ShevaEngine.Core;
using System.Collections.Generic;
using System.Linq;


namespace Microsoft.Xna.Framework.Graphics
{
    public static class ModelMeshExtensions
    {
        /// <summary>
        /// Set cast shadows.
        /// </summary>
        public static void CastShadows(this ModelMesh modelMesh, bool castShadows)
        {
            foreach (Effect effect in modelMesh.Effects)
                if (effect is Material material)
                    material.CastShadows = castShadows;
        }

        /// <summary>
        /// Set receive shadows.
        /// </summary>
        public static void ReceiveShadows(this ModelMesh modelMesh, bool castShadows)
        {
            foreach (Effect effect in modelMesh.Effects)
                if (effect is Material material)
                    material.ReceiveShadows = castShadows;
        }

        /// <summary>
        /// Create model mesh.
        /// </summary>		
        public static ModelMesh CreateModelMesh<T, U>(IEnumerable<T> vertices, IEnumerable<U> indices, Material material = null)
            where T : struct
            where U : struct
        {
            GraphicsDevice device = ShevaGame.Instance.GraphicsDevice;

            VertexBuffer vertexBuffer = new VertexBuffer(device, typeof(T), vertices.Count(), BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices.ToArray());
            MaterialsManager.UpdateVertexDeclarationTag(vertexBuffer.VertexDeclaration);

            IndexBuffer indexBuffer = new IndexBuffer(device,
                typeof(U) == typeof(ushort) || typeof(U) == typeof(short) ?
                    IndexElementSize.SixteenBits :
                    IndexElementSize.ThirtyTwoBits,
                indices.Count(), BufferUsage.WriteOnly);
            indexBuffer.SetData(indices.ToArray());

            ModelMesh modelMesh = new ModelMesh(device, new List<ModelMeshPart>
            {
                new ModelMeshPart()
                {
                    IndexBuffer = indexBuffer,
                    VertexBuffer = vertexBuffer,
                    NumVertices = vertices.Count(),
                    PrimitiveCount = indices.Count() / 3,
                    StartIndex = 0,
                    VertexOffset = 0,
                }
            });

            if (material != null)
                modelMesh.MeshParts[0].Effect = material;

            return modelMesh;
        }

        /// <summary>
        /// Set material.
        /// </summary>
        public static void SetMaterial(this ModelMesh modelMesh, Material material)
        {
            foreach (ModelMeshPart part in modelMesh.MeshParts)
            {
                part.Effect = material;
            }
        }
    }
}
