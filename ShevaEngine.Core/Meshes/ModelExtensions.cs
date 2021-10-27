using ShevaEngine.Core;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Xna.Framework.Graphics
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Set cast shadows.
        /// </summary>
        public static void CastShadows(this Model model, bool castShadows)
        {
            foreach (ModelMesh modelMesh in model.Meshes)
                modelMesh.CastShadows(castShadows);
        }

        /// <summary>
        /// Set receive shadows.
        /// </summary>
        public static void ReceiveShadows(this Model model, bool castShadows)
        {
            foreach (ModelMesh modelMesh in model.Meshes)
                modelMesh.ReceiveShadows(castShadows);
        }

        /// <summary>
        /// Get bounds.
        /// </summary>
        public static BoundingBox GetBounds(this Model model)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    int vertexDataSize = vertexBufferSize / sizeof(float);
                    float[] vertexData = new float[vertexDataSize];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    for (int i = 0; i < vertexDataSize; i += vertexStride / sizeof(float))
                    {
                        Vector3 vertex = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);
                        min = Vector3.Min(min, vertex);
                        max = Vector3.Max(max, vertex);
                    }
                }
            }

            return new BoundingBox(min, max);
        }

        /// <summary>
        /// Create model.
        /// </summary>
        public static Model CreateModel<T, U>(IEnumerable<T> vertices, IEnumerable<U> indices, Material material = null)
            where T : struct
            where U : struct
        {
            return new Model(
                ShevaGame.Instance.GraphicsDevice,
                new List<ModelBone>(),
                new List<ModelMesh>
                {
                    ModelMeshExtensions.CreateModelMesh(vertices, indices, material)
                });
        }

        /// <summary>
        /// Create model.
        /// </summary>
        public static Model CreateModel<T, U>(IEnumerable<(IEnumerable<T> vertices, IEnumerable<U> indices, Material material)> meshes)
            where T : struct
            where U : struct
        {
            return new Model(
                ShevaGame.Instance.GraphicsDevice,
                new List<ModelBone>(),
                meshes.Select(item => ModelMeshExtensions.CreateModelMesh(item.vertices, item.indices, item.material)).ToList());
        }

        /// <summary>
        /// Set material.
        /// </summary>
        public static void SetMaterial(this Model model, Material material)
        {
            foreach (ModelMesh mesh in model.Meshes)
                mesh.SetMaterial(material);
        }
    }
}
