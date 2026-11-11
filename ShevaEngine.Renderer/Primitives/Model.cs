using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Renderer.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShevaEngine.Renderer.Primitives
{
    /// <summary>
    /// Represents a 3D model composed of multiple meshes.
    /// This is a simple model class that doesn't include material settings,
    /// suitable for rendering with a single effect or with materials applied at render time.
    /// </summary>
    public class Model : IDisposable
    {
        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Gets the list of meshes in this model.
        /// </summary>
        public List<Mesh> Meshes { get; } = new List<Mesh>();

        /// <summary>
        /// Gets the number of meshes.
        /// </summary>
        public int MeshCount => Meshes.Count;

        /// <summary>
        /// Gets the total number of vertices across all meshes.
        /// </summary>
        public int TotalVertexCount => Meshes.Sum(m => m.VertexCount);

        /// <summary>
        /// Gets the total number of triangles across all meshes.
        /// </summary>
        public int TotalTriangleCount => Meshes.Sum(m => m.TriangleCount);

        /// <summary>
        /// Gets or sets the world transformation matrix.
        /// </summary>
        public Matrix WorldTransform { get; set; } = Matrix.Identity;

        /// <summary>
        /// Gets the bounding box in world space (combined from all meshes).
        /// </summary>
        public BoundingBox WorldBoundingBox
        {
            get
            {
                if (Meshes.Count == 0)
                    return new BoundingBox(Vector3.Zero, Vector3.Zero);

                BoundingBox combined = Meshes[0].WorldBoundingBox;
                for (int i = 1; i < Meshes.Count; i++)
                {
                    combined = BoundingBox.CreateMerged(combined, Meshes[i].WorldBoundingBox);
                }
                return combined;
            }
        }

        /// <summary>
        /// Gets the bounding sphere in world space (combined from all meshes).
        /// </summary>
        public BoundingSphere WorldBoundingSphere
        {
            get
            {
                if (Meshes.Count == 0)
                    return new BoundingSphere(Vector3.Zero, 0);

                BoundingSphere combined = Meshes[0].WorldBoundingSphere;
                for (int i = 1; i < Meshes.Count; i++)
                {
                    combined = BoundingSphere.CreateMerged(combined, Meshes[i].WorldBoundingSphere);
                }
                return combined;
            }
        }

        /// <summary>
        /// Gets or sets the name of the model.
        /// </summary>
        public string Name { get; set; } = "Unnamed Model";

        /// <summary>
        /// Gets whether the model has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets a user-defined tag object.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Event raised when the model is disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Creates a new empty model.
        /// </summary>
        public Model(GraphicsDevice graphicsDevice, string name = "Unnamed Model")
        {
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            Name = name;
        }

        /// <summary>
        /// Creates a new model with the specified meshes.
        /// </summary>
        public Model(GraphicsDevice graphicsDevice, params Mesh[] meshes) : this(graphicsDevice)
        {
            if (meshes != null)
            {
                foreach (var mesh in meshes)
                {
                    AddMesh(mesh);
                }
            }
        }

        /// <summary>
        /// Creates a new model with a single mesh.
        /// </summary>
        public Model(GraphicsDevice graphicsDevice, Mesh mesh, string name = "Unnamed Model") : this(graphicsDevice, name)
        {
            AddMesh(mesh);
        }

        /// <summary>
        /// Adds a mesh to the model.
        /// </summary>
        public void AddMesh(Mesh mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));

            Meshes.Add(mesh);
        }

        /// <summary>
        /// Removes a mesh from the model.
        /// </summary>
        public bool RemoveMesh(Mesh mesh)
        {
            return Meshes.Remove(mesh);
        }

        /// <summary>
        /// Removes a mesh at the specified index.
        /// </summary>
        public void RemoveMeshAt(int index)
        {
            if (index < 0 || index >= Meshes.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            Meshes.RemoveAt(index);
        }

        /// <summary>
        /// Clears all meshes from the model.
        /// </summary>
        public void ClearMeshes()
        {
            foreach (var mesh in Meshes)
            {
                mesh.Dispose();
            }
            Meshes.Clear();
        }

        /// <summary>
        /// Gets a mesh by name.
        /// </summary>
        public Mesh GetMeshByName(string name)
        {
            return Meshes.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Creates a simple cube model.
        /// </summary>
        public static Model CreateCube(GraphicsDevice graphicsDevice, float size = 1.0f, string name = "Cube")
        {
            var mesh = Mesh.CreateCube(graphicsDevice, size, name);
            return new Model(graphicsDevice, mesh, name);
        }

        /// <summary>
        /// Creates a simple sphere model.
        /// </summary>
        public static Model CreateSphere(GraphicsDevice graphicsDevice, float radius = 1.0f, int slices = 16, int stacks = 16, string name = "Sphere")
        {
            var mesh = Mesh.CreateSphere(graphicsDevice, radius, slices, stacks, name);
            return new Model(graphicsDevice, mesh, name);
        }

        /// <summary>
        /// Creates a simple plane model.
        /// </summary>
        public static Model CreatePlane(GraphicsDevice graphicsDevice, float width = 1.0f, float depth = 1.0f, int widthSegments = 1, int depthSegments = 1, string name = "Plane")
        {
            var mesh = Mesh.CreatePlane(graphicsDevice, width, depth, widthSegments, depthSegments, name);
            return new Model(graphicsDevice, mesh, name);
        }

        /// <summary>
        /// Creates a simple quad model.
        /// </summary>
        public static Model CreateQuad(GraphicsDevice graphicsDevice, float size = 1.0f, string name = "Quad")
        {
            var mesh = Mesh.CreateQuad(graphicsDevice, size, name);
            return new Model(graphicsDevice, mesh, name);
        }

        /// <summary>
        /// Draws all meshes in the model using the specified effect and world matrix.
        /// </summary>
        public void Draw(Effect effect, Matrix worldMatrix)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            // Set world matrix
            EffectParameter worldParam = effect.Parameters["World"] ??
                                          effect.Parameters["WorldMatrix"] ??
                                          effect.Parameters["Transform"];
            if (worldParam != null)
            {
                worldParam.SetValue(worldMatrix);
            }

            // Draw each mesh
            foreach (var mesh in Meshes)
            {
                mesh.Draw(effect, worldMatrix);
            }
        }

        /// <summary>
        /// Draws all meshes in the model using the specified effect and the model's world transform.
        /// </summary>
        public void Draw(Effect effect)
        {
            Draw(effect, WorldTransform);
        }

        /// <summary>
        /// Draws all meshes in the model using the specified effect, with per-mesh world transforms.
        /// </summary>
        public void Draw(Effect effect, Matrix[] meshWorldMatrices)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));
            if (meshWorldMatrices == null || meshWorldMatrices.Length != Meshes.Count)
                throw new ArgumentException("Mesh world matrices count must match mesh count");

            for (int i = 0; i < Meshes.Count; i++)
            {
                Meshes[i].Draw(effect, meshWorldMatrices[i]);
            }
        }

        /// <summary>
        /// Draws the model with instancing.
        /// </summary>
        public void DrawInstanced(Effect effect, Matrix[] instanceWorldMatrices)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            // For instanced rendering, we would typically use a different approach
            // This is a simplified version
            foreach (var worldMatrix in instanceWorldMatrices)
            {
                Draw(effect, worldMatrix);
            }
        }

        /// <summary>
        /// Updates the world transform for all meshes.
        /// </summary>
        public void UpdateMeshTransforms()
        {
            foreach (var mesh in Meshes)
            {
                mesh.WorldTransform = WorldTransform * mesh.LocalTransform;
            }
        }

        /// <summary>
        /// Disposes the model and all its meshes.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            ClearMeshes();
            IsDisposed = true;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Returns a string representation of the model.
        /// </summary>
        public override string ToString()
        {
            return $"{Name} (Meshes: {MeshCount}, Vertices: {TotalVertexCount}, Triangles: {TotalTriangleCount})";
        }

        /// <summary>
        /// Finalizer to ensure proper disposal.
        /// </summary>
        ~Model()
        {
            Dispose();
        }

        /// <summary>
        /// Enumerates through all meshes.
        /// </summary>
        public IEnumerator<Mesh> GetEnumerator()
        {
            return Meshes.GetEnumerator();
        }

        /// <summary>
        /// Gets a mesh by index.
        /// </summary>
        public Mesh this[int index]
        {
            get => Meshes[index];
            set => Meshes[index] = value;
        }

        /// <summary>
        /// Creates a model from vertex and index arrays.
        /// </summary>
        public static Model FromVertexAndIndexArrays(GraphicsDevice graphicsDevice, PBRVertex[] vertices, int[] indices, string name = "Custom Model")
        {
            var mesh = new Mesh(graphicsDevice, vertices, indices, name);
            return new Model(graphicsDevice, mesh, name);
        }

        /// <summary>
        /// Creates a model from multiple vertex and index arrays (one mesh per array pair).
        /// </summary>
        public static Model FromMultipleMeshes(GraphicsDevice graphicsDevice, params (PBRVertex[] vertices, int[] indices, string name)[] meshData)
        {
            var model = new Model(graphicsDevice);
            foreach (var data in meshData)
            {
                var mesh = new Mesh(graphicsDevice, data.vertices, data.indices, data.name);
                model.AddMesh(mesh);
            }
            return model;
        }
    }
}
