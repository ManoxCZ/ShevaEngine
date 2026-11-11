using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ShevaEngine.Renderer.Primitives
{
    /// <summary>
    /// Vertex structure for PBR (Physically Based Rendering) with normal mapping support.
    /// Contains Position, Normal, Tangent, Binormal, and Texture Coordinates.
    /// This matches the VertexShaderInputPNTTB structure in the PBR.fx shader.
    /// </summary>
    public struct PBRVertex : IVertexType
    {
        /// <summary>
        /// Vertex position in model space.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Vertex normal.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Vertex tangent for normal mapping.
        /// </summary>
        public Vector3 Tangent;

        /// <summary>
        /// Vertex binormal (bitangent) for normal mapping.
        /// </summary>
        public Vector3 Binormal;

        /// <summary>
        /// Texture coordinates (UV) for the first texture set.
        /// </summary>
        public Vector2 TextureCoordinate;

        /// <summary>
        /// Static vertex declaration for PBRVertex.
        /// </summary>
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
            new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
            new VertexElement(48, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        /// <summary>
        /// Creates a new PBR vertex.
        /// </summary>
        public PBRVertex(Vector3 position, Vector3 normal, Vector3 tangent, Vector3 binormal, Vector2 textureCoordinate)
        {
            Position = position;
            Normal = normal;
            Tangent = tangent;
            Binormal = binormal;
            TextureCoordinate = textureCoordinate;
        }

        /// <summary>
        /// Gets the vertex declaration.
        /// </summary>
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        /// <summary>
        /// Size of the vertex in bytes.
        /// </summary>
        public const int SizeInBytes = 12 + 12 + 12 + 12 + 8; // 56 bytes

        /// <summary>
        /// Creates a PBR vertex with default tangent and binormal calculated from normal.
        /// This is useful for simple meshes where you want to calculate tangent space automatically.
        /// </summary>
        public static PBRVertex CreateWithDefaultTangentSpace(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
        {
            // Default tangent and binormal for simple cases
            // This creates a basic tangent space, but for proper normal mapping,
            // you should calculate tangent and binormal from the mesh geometry
            Vector3 tangent = Vector3.UnitX;
            Vector3 binormal = Vector3.UnitZ;

            // Orthogonalize
            if (Math.Abs(Vector3.Dot(normal, Vector3.UnitY)) > 0.9f)
            {
                // If normal is close to Y axis, use different defaults
                tangent = Vector3.UnitX;
                binormal = Vector3.UnitZ;
            }
            else
            {
                tangent = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, normal));
                binormal = Vector3.Cross(normal, tangent);
            }

            return new PBRVertex(position, normal, tangent, binormal, textureCoordinate);
        }

        /// <summary>
        /// Calculates tangent and binormal from position and normal using Mikktspace algorithm.
        /// This is a simplified version for pre-calculated meshes.
        /// </summary>
        public static void CalculateTangentSpace(
            PBRVertex[] vertices,
            int[] indices,
            int vertexCount,
            int indexCount)
        {
            // This is a simplified tangent space calculation
            // For production use, consider using a proper Mikktspace implementation
            // or calculating tangents during mesh creation

            // First pass: accumulate tangent and binormal contributions
            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];

            for (int i = 0; i < indexCount; i += 3)
            {
                int i1 = indices[i];
                int i2 = indices[i + 1];
                int i3 = indices[i + 2];

                Vector3 v1 = vertices[i1].Position;
                Vector3 v2 = vertices[i2].Position;
                Vector3 v3 = vertices[i3].Position;

                Vector2 w1 = vertices[i1].TextureCoordinate;
                Vector2 w2 = vertices[i2].TextureCoordinate;
                Vector2 w3 = vertices[i3].TextureCoordinate;

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float r = 1.0f / (s1 * t2 - s2 * t1);

                Vector3 sdir = new Vector3(
                    (t2 * x1 - t1 * x2) * r,
                    (t2 * y1 - t1 * y2) * r,
                    (t2 * z1 - t1 * z2) * r);

                Vector3 tdir = new Vector3(
                    (s1 * x2 - s2 * x1) * r,
                    (s1 * y2 - s2 * y1) * r,
                    (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            // Second pass: calculate final tangent and binormal
            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 normal = vertices[i].Normal;
                Vector3 tangent = tan1[i];

                // Gram-Schmidt orthogonalization
                tangent = Vector3.Normalize(tangent - Vector3.Dot(tangent, normal) * normal);

                // Calculate binormal as cross product of normal and tangent
                // Use the original tan2 to determine the direction (flip if needed)
                float dot = Vector3.Dot(Vector3.Cross(normal, tangent), tan2[i]);
                float w = (dot > 0.0f) ? 1.0f : -1.0f;

                vertices[i].Tangent = tangent * w;
                vertices[i].Binormal = Vector3.Cross(normal, tangent) * w;
            }
        }

        /// <summary>
        /// Creates a simple quad mesh with PBR vertices.
        /// </summary>
        public static PBRVertex[] CreateQuad(float size = 1.0f)
        {
            Vector3 halfSize = new Vector3(size / 2.0f);

            PBRVertex[] vertices = new PBRVertex[4];

            // Bottom-left
            vertices[0] = new PBRVertex(
                new Vector3(-halfSize.X, -halfSize.Y, 0),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(0, 0)
            );

            // Bottom-right
            vertices[1] = new PBRVertex(
                new Vector3(halfSize.X, -halfSize.Y, 0),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(1, 0)
            );

            // Top-right
            vertices[2] = new PBRVertex(
                new Vector3(halfSize.X, halfSize.Y, 0),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(1, 1)
            );

            // Top-left
            vertices[3] = new PBRVertex(
                new Vector3(-halfSize.X, halfSize.Y, 0),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(0, 1)
            );

            return vertices;
        }

        /// <summary>
        /// Creates a cube mesh with PBR vertices.
        /// </summary>
        public static PBRVertex[] CreateCube(float size = 1.0f)
        {
            Vector3 halfSize = new Vector3(size / 2.0f);
            PBRVertex[] vertices = new PBRVertex[24];

            // Front face
            vertices[0] = new PBRVertex(
                new Vector3(-halfSize.X, -halfSize.Y, halfSize.Z),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(0, 0)
            );
            vertices[1] = new PBRVertex(
                new Vector3(halfSize.X, -halfSize.Y, halfSize.Z),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(1, 0)
            );
            vertices[2] = new PBRVertex(
                new Vector3(halfSize.X, halfSize.Y, halfSize.Z),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(1, 1)
            );
            vertices[3] = new PBRVertex(
                new Vector3(-halfSize.X, halfSize.Y, halfSize.Z),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(0, 1)
            );

            // Back face
            vertices[4] = new PBRVertex(
                new Vector3(-halfSize.X, -halfSize.Y, -halfSize.Z),
                new Vector3(0, 0, -1),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(1, 0)
            );
            vertices[5] = new PBRVertex(
                new Vector3(halfSize.X, -halfSize.Y, -halfSize.Z),
                new Vector3(0, 0, -1),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(0, 0)
            );
            vertices[6] = new PBRVertex(
                new Vector3(halfSize.X, halfSize.Y, -halfSize.Z),
                new Vector3(0, 0, -1),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(0, 1)
            );
            vertices[7] = new PBRVertex(
                new Vector3(-halfSize.X, halfSize.Y, -halfSize.Z),
                new Vector3(0, 0, -1),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector2(1, 1)
            );

            // Left face
            vertices[8] = new PBRVertex(
                new Vector3(-halfSize.X, -halfSize.Y, -halfSize.Z),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 0),
                new Vector2(0, 0)
            );
            vertices[9] = new PBRVertex(
                new Vector3(-halfSize.X, -halfSize.Y, halfSize.Z),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 0),
                new Vector2(1, 0)
            );
            vertices[10] = new PBRVertex(
                new Vector3(-halfSize.X, halfSize.Y, halfSize.Z),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 0),
                new Vector2(1, 1)
            );
            vertices[11] = new PBRVertex(
                new Vector3(-halfSize.X, halfSize.Y, -halfSize.Z),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 0),
                new Vector2(0, 1)
            );

            // Right face
            vertices[12] = new PBRVertex(
                new Vector3(halfSize.X, -halfSize.Y, halfSize.Z),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, -1),
                new Vector3(0, 1, 0),
                new Vector2(0, 0)
            );
            vertices[13] = new PBRVertex(
                new Vector3(halfSize.X, -halfSize.Y, -halfSize.Z),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, -1),
                new Vector3(0, 1, 0),
                new Vector2(1, 0)
            );
            vertices[14] = new PBRVertex(
                new Vector3(halfSize.X, halfSize.Y, -halfSize.Z),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, -1),
                new Vector3(0, 1, 0),
                new Vector2(1, 1)
            );
            vertices[15] = new PBRVertex(
                new Vector3(halfSize.X, halfSize.Y, halfSize.Z),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, -1),
                new Vector3(0, 1, 0),
                new Vector2(0, 1)
            );

            // Top face
            vertices[16] = new PBRVertex(
                new Vector3(-halfSize.X, halfSize.Y, halfSize.Z),
                new Vector3(0, 1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, -1),
                new Vector2(0, 0)
            );
            vertices[17] = new PBRVertex(
                new Vector3(halfSize.X, halfSize.Y, halfSize.Z),
                new Vector3(0, 1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, -1),
                new Vector2(1, 0)
            );
            vertices[18] = new PBRVertex(
                new Vector3(halfSize.X, halfSize.Y, -halfSize.Z),
                new Vector3(0, 1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, -1),
                new Vector2(1, 1)
            );
            vertices[19] = new PBRVertex(
                new Vector3(-halfSize.X, halfSize.Y, -halfSize.Z),
                new Vector3(0, 1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, -1),
                new Vector2(0, 1)
            );

            // Bottom face
            vertices[20] = new PBRVertex(
                new Vector3(-halfSize.X, -halfSize.Y, -halfSize.Z),
                new Vector3(0, -1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector2(1, 0)
            );
            vertices[21] = new PBRVertex(
                new Vector3(halfSize.X, -halfSize.Y, -halfSize.Z),
                new Vector3(0, -1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector2(0, 0)
            );
            vertices[22] = new PBRVertex(
                new Vector3(halfSize.X, -halfSize.Y, halfSize.Z),
                new Vector3(0, -1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector2(0, 1)
            );
            vertices[23] = new PBRVertex(
                new Vector3(-halfSize.X, -halfSize.Y, halfSize.Z),
                new Vector3(0, -1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector2(1, 1)
            );

            // Calculate tangent space for proper normal mapping
            int[] cubeIndices = GetCubeIndices();
            CalculateTangentSpace(vertices, cubeIndices, vertices.Length, cubeIndices.Length);

            return vertices;
        }

        /// <summary>
        /// Gets the indices for a cube (12 triangles, 36 indices).
        /// </summary>
        public static int[] GetCubeIndices()
        {
            return new int[]
            {
                // Front face
                0, 1, 2,
                0, 2, 3,
                // Back face
                4, 6, 5,
                4, 7, 6,
                // Left face
                8, 10, 9,
                8, 11, 10,
                // Right face
                12, 14, 13,
                12, 15, 14,
                // Top face
                16, 18, 17,
                16, 19, 18,
                // Bottom face
                20, 22, 21,
                20, 23, 22
            };
        }

        /// <summary>
        /// Creates a sphere mesh with PBR vertices.
        /// </summary>
        public static (PBRVertex[] vertices, int[] indices) CreateSphere(float radius = 1.0f, int slices = 16, int stacks = 16)
        {
            List<PBRVertex> verticesList = new List<PBRVertex>();
            List<int> indicesList = new List<int>();

            for (int i = 0; i <= stacks; i++)
            {
                float phi = MathHelper.Pi * i / stacks;
                for (int j = 0; j <= slices; j++)
                {
                    float theta = 2 * MathHelper.Pi * j / slices;

                    Vector3 position = new Vector3(
                        (float)(radius * Math.Sin(phi) * Math.Cos(theta)),
                        (float)(radius * Math.Cos(phi)),
                        (float)(radius * Math.Sin(phi) * Math.Sin(theta))
                    );

                    Vector3 normal = Vector3.Normalize(position);

                    Vector2 textureCoordinate = new Vector2(
                        (float)(j / (float)slices),
                        (float)(i / (float)stacks)
                    );

                    // Temporary tangent and binormal (will be recalculated)
                    verticesList.Add(new PBRVertex(position, normal, Vector3.UnitX, Vector3.UnitZ, textureCoordinate));
                }
            }

            for (int i = 0; i < stacks; i++)
            {
                for (int j = 0; j < slices; j++)
                {
                    int first = i * (slices + 1) + j;
                    int second = first + slices + 1;

                    indicesList.Add(first);
                    indicesList.Add(second);
                    indicesList.Add(first + 1);

                    indicesList.Add(first + 1);
                    indicesList.Add(second);
                    indicesList.Add(second + 1);
                }
            }

            PBRVertex[] vertices = verticesList.ToArray();
            int[] indices = indicesList.ToArray();

            // Calculate proper tangent space
            CalculateTangentSpace(vertices, indices, vertices.Length, indices.Length);

            return (vertices, indices);
        }
    }
}
