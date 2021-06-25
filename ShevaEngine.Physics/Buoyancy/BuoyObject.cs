using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using ShevaEngine.Physics;


namespace ShevaEngine.Physics
{
    public class BuoyObject
    {
        public RigidBody RigidBody { get; private set; }
        public List<Buoy> Buoys { get; private set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public BuoyObject()
        {
            Buoys = new List<Buoy>();
        }

        public void LoadContent(ShevaGame game)
        {
            Model model = game.Content.Load<Model>("Content\\Models\\hull");
            Shape shape = GetShape(model);

            RigidBody = new RigidBody(shape)
            {
                Mass = 5000f
            };
            Buoy buoy = new Buoy();
            buoy.CreateHullTriangles(model);

            Buoys.Add(buoy);            
        }

        private static TriangleMeshShape GetShape(Model model)
        {
            if (model.Meshes.Count != 1)
                return null;
            
            if (model.Meshes[0].MeshParts.Count != 1)
                return null;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[model.Meshes[0].MeshParts[0].NumVertices];
            model.Meshes[0].MeshParts[0].VertexBuffer.GetData(vertices);

            ushort[] indices = new ushort[model.Meshes[0].MeshParts[0].IndexBuffer.IndexCount];
            model.Meshes[0].MeshParts[0].IndexBuffer.GetData(indices);

            List<JVector> triangles = new List<JVector>();
            List<TriangleVertexIndices> triangleIndices = new List<TriangleVertexIndices>();

            for (int i = 0; i < model.Meshes[0].MeshParts[0].PrimitiveCount; i++)
            {
                triangles.Add(new JVector(vertices[indices[3 * i]].Position));
                triangles.Add(new JVector(vertices[indices[3 * i + 1]].Position));
                triangles.Add(new JVector(vertices[indices[3 * i + 2]].Position));
                triangleIndices.Add(new TriangleVertexIndices(3 * i, 3 * i + 1, 3 * i + 2));
            }

            TriangleMeshShape meshShape = new TriangleMeshShape(new Octree(triangles, triangleIndices));
            meshShape.MakeHull(ref triangles, 0);

            return meshShape;
        }

        public virtual void UpdatePhysics(GameTime time)
        {
            foreach (Buoy buoy in Buoys)
                buoy.UpdatePhysics(time, RigidBody);
        }

        internal float GetCurrentVolume()
        {
            return -1;
            //return Enumerable.Sum<Buoy>((IEnumerable<Buoy>)(object)Buoys, (Func<Buoy, float>)((Buoy item) => item.CurrentVolume));
        }

        private void GetWaterLevel()
        {
        }
    }
}