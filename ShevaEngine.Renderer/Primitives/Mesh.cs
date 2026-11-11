using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Renderer.Primitives;
using System;
using System.Collections.Generic;

namespace ShevaEngine.Renderer.Primitives
{
    /// <summary>
    /// Represents a single mesh with PBR vertices.
    /// Contains vertex and index buffers for rendering.
    /// </summary>
    public class Mesh : IDisposable
    {
        /// <summary>
        /// Gets the vertex buffer.
        /// </summary>
        public VertexBuffer VertexBuffer { get; private set; }

        /// <summary>
        /// Gets the index buffer.
        /// </summary>
        public IndexBuffer IndexBuffer { get; private set; }

        /// <summary>
        /// Gets the number of vertices.
        /// </summary>
        public int VertexCount { get; private set; }

        /// <summary>
        /// Gets the number of indices.
        /// </summary>
        public int IndexCount { get; private set; }

        /// <summary>
        /// Gets the number of triangles.
        /// </summary>
        public int TriangleCount => IndexCount / 3;

        /// <summary>
        /// Gets or sets the vertex data.
        /// </summary>
        public PBRVertex[] Vertices { get; private set; }

        /// <summary>
        /// Gets or sets the index data.
        /// </summary>
        public int[] Indices { get; private set; }

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Gets or sets the local transformation matrix.
        /// </summary>
        public Matrix LocalTransform { get; set; } = Matrix.Identity;

        /// <summary>
        /// Gets or sets the world transformation matrix.
        /// </summary>
        public Matrix WorldTransform { get; set; } = Matrix.Identity;

        /// <summary>
        /// Gets the bounding box in local space.
        /// </summary>
        public BoundingBox LocalBoundingBox { get; private set; }

        /// <summary>
        /// Gets the bounding box in world space.
        /// </summary>
        public BoundingBox WorldBoundingBox => LocalBoundingBox.Transform(WorldTransform);

        /// <summary>
        /// Gets the bounding sphere in local space.
        /// </summary>
        public BoundingSphere LocalBoundingSphere { get; private set; }

        /// <summary>
        /// Gets the bounding sphere in world space.
        /// </summary>
        public BoundingSphere WorldBoundingSphere => LocalBoundingSphere.Transform(WorldTransform);

        /// <summary>
        /// Gets or sets the name of the mesh.
        /// </summary>
        public string Name { get; set; } = "Unnamed Mesh";

        /// <summary>
        /// Gets whether the mesh has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets a user-defined tag object.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Event raised when the mesh is disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Creates a new mesh with the specified vertices and indices.
        /// </summary>
        public Mesh(GraphicsDevice graphicsDevice, PBRVertex[] vertices, int[] indices, string name = "Unnamed Mesh")
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));
            if (vertices == null || vertices.Length == 0)
                throw new ArgumentException("Vertices cannot be null or empty", nameof(vertices));

            GraphicsDevice = graphicsDevice;
            Vertices = vertices;
            Indices = indices ?? CreateDefaultIndices(vertices.Length);
            VertexCount = vertices.Length;
            IndexCount = Indices.Length;
            Name = name;

            // Create vertex and index buffers
            CreateBuffers();

            // Calculate bounding volumes
            CalculateBoundingVolumes();
        }

        /// <summary>
        /// Creates a new mesh with the specified vertex and index buffers.
        /// </summary>
        public Mesh(GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, string name = "Unnamed Mesh")
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));
            if (vertexBuffer == null)
                throw new ArgumentNullException(nameof(vertexBuffer));

            GraphicsDevice = graphicsDevice;
            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
            VertexCount = vertexBuffer.VertexCount;
            IndexCount = indexBuffer.IndexCount;
            Name = name;

            // Calculate bounding volumes from buffer data
            CalculateBoundingVolumesFromBuffers();
        }

        /// <summary>
        /// Creates default indices for a triangle list (assuming vertices form a triangle strip).
        /// </summary>
        private static int[] CreateDefaultIndices(int vertexCount)
        {
            // For now, just create indices that match vertex order
            // This should be overridden by proper indices in most cases
            int[] indices = new int[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                indices[i] = i;
            }
            return indices;
        }

        /// <summary>
        /// Creates the vertex and index buffers.
        /// </summary>
        private void CreateBuffers()
        {
            // Vertex buffer
            VertexBuffer = new VertexBuffer(
                GraphicsDevice,
                PBRVertex.VertexDeclaration,
                VertexCount,
                BufferUsage.WriteOnly
            );
            VertexBuffer.SetData(Vertices);

            // Index buffer
            if (Indices != null && Indices.Length > 0)
            {
                IndexBuffer = new IndexBuffer(
                    GraphicsDevice,
                    IndexElementSize.ThirtyTwoBits,
                    IndexCount,
                    BufferUsage.WriteOnly
                );
                IndexBuffer.SetData(Indices);
            }
        }

        /// <summary>
        /// Calculates the bounding box and bounding sphere from vertex data.
        /// </summary>
        private void CalculateBoundingVolumes()
        {
            if (Vertices == null || Vertices.Length == 0)
            {
                LocalBoundingBox = new BoundingBox(Vector3.Zero, Vector3.Zero);
                LocalBoundingSphere = new BoundingSphere(Vector3.Zero, 0);
                return;
            }

            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (var vertex in Vertices)
            {
                min = Vector3.Min(min, vertex.Position);
                max = Vector3.Max(max, vertex.Position);
            }

            LocalBoundingBox = new BoundingBox(min, max);

            // Calculate bounding sphere
            Vector3 center = (min + max) / 2;
            float radius = 0;
            foreach (var vertex in Vertices)
            {
                float distance = Vector3.Distance(center, vertex.Position);
                radius = Math.Max(radius, distance);
            }
            LocalBoundingSphere = new BoundingSphere(center, radius);
        }

        /// <summary>
        /// Calculates bounding volumes from buffer data.
        /// </summary>
        private void CalculateBoundingVolumesFromBuffers()
        {
            // Get vertex data from buffer
            PBRVertex[] vertices = new PBRVertex[VertexCount];
            VertexBuffer.GetData(vertices);

            // Get index data if available
            int[] indices = null;
            if (IndexBuffer != null)
            {
                indices = new int[IndexCount];
                IndexBuffer.GetData(indices);
            }

            Vertices = vertices;
            Indices = indices;
            VertexCount = vertices.Length;
            IndexCount = indices?.Length ?? 0;

            CalculateBoundingVolumes();
        }

        /// <summary>
        /// Creates a simple quad mesh.
        /// </summary>
        public static Mesh CreateQuad(GraphicsDevice graphicsDevice, float size = 1.0f, string name = "Quad")
        {
            PBRVertex[] vertices = PBRVertex.CreateQuad(size);
            int[] indices = new int[] { 0, 1, 2, 0, 2, 3 };
            return new Mesh(graphicsDevice, vertices, indices, name);
        }

        /// <summary>
        /// Creates a cube mesh.
        /// </summary>
        public static Mesh CreateCube(GraphicsDevice graphicsDevice, float size = 1.0f, string name = "Cube")
        {
            PBRVertex[] vertices = PBRVertex.CreateCube(size);
            int[] indices = PBRVertex.GetCubeIndices();
            return new Mesh(graphicsDevice, vertices, indices, name);
        }

        /// <summary>
        /// Creates a sphere mesh.
        /// </summary>
        public static Mesh CreateSphere(GraphicsDevice graphicsDevice, float radius = 1.0f, int slices = 16, int stacks = 16, string name = "Sphere")
        {
            var result = PBRVertex.CreateSphere(radius, slices, stacks);
            return new Mesh(graphicsDevice, result.vertices, result.indices, name);
        }

        /// <summary>
        /// Creates a plane mesh (grid).
        /// </summary>
        public static Mesh CreatePlane(GraphicsDevice graphicsDevice, float width = 1.0f, float depth = 1.0f, int widthSegments = 1, int depthSegments = 1, string name = "Plane")
        {
            List<PBRVertex> vertices = new List<PBRVertex>();
            List<int> indices = new List<int>();

            float halfWidth = width / 2.0f;
            float halfDepth = depth / 2.0f;
            float segmentWidth = width / widthSegments;
            float segmentDepth = depth / depthSegments;

            // Create vertices
            for (int z = 0; z <= depthSegments; z++)
            {
                for (int x = 0; x <= widthSegments; x++)
                {
                    float xPos = -halfWidth + x * segmentWidth;
                    float zPos = -halfDepth + z * segmentDepth;

                    Vector3 position = new Vector3(xPos, 0, zPos);
                    Vector3 normal = Vector3.Up;
                    Vector2 uv = new Vector2(
                        (float)x / widthSegments,
                        (float)z / depthSegments
                    );

                    vertices.Add(new PBRVertex(position, normal, Vector3.Right, Vector3.Forward, uv));
                }
            }

            // Create indices
            for (int z = 0; z < depthSegments; z++)
            {
                for (int x = 0; x < widthSegments; x++)
                {
                    int topLeft = z * (widthSegments + 1) + x;
                    int topRight = topLeft + 1;
                    int bottomLeft = (z + 1) * (widthSegments + 1) + x;
                    int bottomRight = bottomLeft + 1;

                    indices.Add(topLeft);
                    indices.Add(bottomLeft);
                    indices.Add(topRight);

                    indices.Add(topRight);
                    indices.Add(bottomLeft);
                    indices.Add(bottomRight);
                }
            }

            PBRVertex[] vertexArray = vertices.ToArray();
            int[] indexArray = indices.ToArray();

            // Calculate proper tangent space
            PBRVertex.CalculateTangentSpace(vertexArray, indexArray, vertexArray.Length, indexArray.Length);

            return new Mesh(graphicsDevice, vertexArray, indexArray, name);
        }

        /// <summary>
        /// Updates the vertex buffer with new vertex data.
        /// </summary>
        public void UpdateVertices(PBRVertex[] vertices)
        {
            if (vertices == null || vertices.Length != VertexCount)
                throw new ArgumentException("Vertex count must match existing vertex buffer");

            Vertices = vertices;
            VertexBuffer.SetData(vertices);
            CalculateBoundingVolumes();
        }

        /// <summary>
        /// Updates the index buffer with new index data.
        /// </summary>
        public void UpdateIndices(int[] indices)
        {
            if (indices == null)
                throw new ArgumentNullException(nameof(indices));

            Indices = indices;
            IndexCount = indices.Length;

            if (IndexBuffer != null)
            {
                IndexBuffer.SetData(indices);
            }
            else
            {
                IndexBuffer = new IndexBuffer(
                    GraphicsDevice,
                    IndexElementSize.ThirtyTwoBits,
                    IndexCount,
                    BufferUsage.WriteOnly
                );
                IndexBuffer.SetData(indices);
            }
        }

        /// <summary>
        /// Draws the mesh using the specified effect and world matrix.
        /// </summary>
        public void Draw(Effect effect, Matrix worldMatrix)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            GraphicsDevice.SetVertexBuffer(VertexBuffer);
            GraphicsDevice.Indices = IndexBuffer;

            // Set world matrix
            EffectParameter worldParam = effect.Parameters["World"] ??
                                          effect.Parameters["WorldMatrix"] ??
                                          effect.Parameters["Transform"];
            if (worldParam != null)
            {
                worldParam.SetValue(worldMatrix);
            }

            // Draw
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    TriangleCount
                );
            }
        }

        /// <summary>
        /// Draws the mesh using the specified effect with the mesh's world transform.
        /// </summary>
        public void Draw(Effect effect)
        {
            Draw(effect, WorldTransform);
        }

        /// <summary>
        /// Draws the mesh without an effect (uses the currently set effect on the device).
        /// </summary>
        public void DrawInstanced(int instanceCount)
        {
            if (InstanceCount == 0 || IndexCount == 0)
                return;

            GraphicsDevice.SetVertexBuffer(VertexBuffer);
            GraphicsDevice.Indices = IndexBuffer;

            GraphicsDevice.DrawInstancedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                TriangleCount,
                instanceCount
            );
        }

        /// <summary>
        /// Disposes the mesh and releases all resources.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            VertexBuffer?.Dispose();
            VertexBuffer = null;

            IndexBuffer?.Dispose();
            IndexBuffer = null;

            Vertices = null;
            Indices = null;

            IsDisposed = true;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Returns a string representation of the mesh.
        /// </summary>
        public override string ToString()
        {
            return $"{Name} (Vertices: {VertexCount}, Triangles: {TriangleCount})";
        }

        /// <summary>
        /// Finalizer to ensure proper disposal.
        /// </summary>
        ~Mesh()
        {
            Dispose();
        }
    }
}
