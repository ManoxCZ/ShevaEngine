using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Physics;

namespace ShevaEngine.Physics
{
    public class Buoy : Object
    {
        public const float cWaterDensity = 997f;
        public const float cHalfWaterDensity = 498.5f;
        public const float cG = 9.8f;

        public ResistanceType ResistanceType;
        public Vector3 ResistanceFactor;
        public AirResistanceType AirResistanceType;
        public Vector3 AirResistanceFactor;
        private List<HullTriangle> _hullTriangles;
        private List<HullTriangle> _clippedTriangles;
        //internal Vector3 FResistanceCenter;
        //internal Vector3 FResistanceBB;
        //internal Vector3 BoundingBoxArea;


        //public float Length { get; set; }

        //public float CurrentWaterSurface
        //{
        //    [CompilerGenerated]
        //    get
        //    {
        //        return < CurrentWaterSurface > k__BackingField;
        //    }
        //    [CompilerGenerated]
        //    private set
        //    {

        //    < CurrentWaterSurface > k__BackingField = value;
        //    }
        //}

        //public float CurrentVolume
        //{
        //    [CompilerGenerated]
        //    get
        //    {
        //        return < CurrentVolume > k__BackingField;
        //    }
        //    [CompilerGenerated]
        //    private set
        //    {

        //    < CurrentVolume > k__BackingField = value;
        //    }
        //}

        public float TotalVolume { get; set; }
        public Vector3 VolumeCenter { get; set; }

        //public Vector3 BoundingBox
        //{
        //    [CompilerGenerated]
        //    get
        //    {
        //        return < BoundingBox > k__BackingField;
        //    }
        //    [CompilerGenerated]
        //    private set
        //    {

        //    < BoundingBox > k__BackingField = value;
        //    }
        //}

        //public Vector3 BoundingBoxCenter
        //{
        //    [CompilerGenerated]
        //    get
        //    {
        //        return < BoundingBoxCenter > k__BackingField;
        //    }
        //    [CompilerGenerated]
        //    private set
        //    {

        //    < BoundingBoxCenter > k__BackingField = value;
        //    }
        //}


        /// <summary>
        /// Constructor.
        /// </summary>
        public Buoy()
        {
            ResistanceType = ResistanceType.Complex;
            AirResistanceType = AirResistanceType.None;
            ResistanceFactor = new Vector3(0.997f, 0.997f, 0.997f);
            AirResistanceFactor = new Vector3(0.99f, 0.99f, 0.99f);
        }

        /// <summary>
        /// Create hull triangles.
        /// </summary>
        public bool CreateHullTriangles(Model model)
        {
            _hullTriangles = GetHullTriangles(model);

            if (_hullTriangles == null)
                return false;

            Vector3 min = new Vector3(3.40282347E+38f);
            Vector3 max = new Vector3(-3.40282347E+38f);


            foreach (HullTriangle triangle in _hullTriangles)
            {
                min.X = Math.Min(Math.Min(Math.Min(min.X, triangle.Vertex1.X), triangle.Vertex2.X), triangle.Vertex3.X);
                min.Y = Math.Min(Math.Min(Math.Min(min.Y, triangle.Vertex1.Y), triangle.Vertex2.Y), triangle.Vertex3.Y);
                min.Z = Math.Min(Math.Min(Math.Min(min.Z, triangle.Vertex1.Z), triangle.Vertex2.Z), triangle.Vertex3.Z);
                max.X = Math.Max(Math.Max(Math.Max(max.X, triangle.Vertex1.X), triangle.Vertex2.X), triangle.Vertex3.X);
                max.Y = Math.Max(Math.Max(Math.Max(max.Y, triangle.Vertex1.Y), triangle.Vertex2.Y), triangle.Vertex3.Y);
                max.Z = Math.Max(Math.Max(Math.Max(max.Z, triangle.Vertex1.Z), triangle.Vertex2.Z), triangle.Vertex3.Z);
            }

            //BoundingBox = max - min;
            //BoundingBoxCenter = Vector3.Lerp(min, max, 0.5f);
            //BoundingBoxArea = new Vector3(BoundingBox.Y * BoundingBox.Z, BoundingBox.X * BoundingBox.Z, BoundingBox.X * BoundingBox.Y);
            //Length = Math.Max(Math.Max(BoundingBox.X, BoundingBox.Y), BoundingBox.Z);

            _clippedTriangles = new List<HullTriangle>(_hullTriangles.Count * 2);
            _clippedTriangles.AddRange(_hullTriangles);

            TotalVolume = CalcVolume(new Plane(Vector3.Up, 1000f), _clippedTriangles);
            return true;
        }

        /// <summary>
        /// Get hull triangles.
        /// </summary>
        private static List<HullTriangle> GetHullTriangles(Model model)
        {
            List<HullTriangle> result = new List<HullTriangle>();

            if (model.Meshes.Count != 1)
                return null;

            if (model.Meshes[0].MeshParts.Count != 1)
                return null;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[model.Meshes[0].MeshParts[0].NumVertices];
            model.Meshes[0].MeshParts[0].VertexBuffer.GetData(vertices);

            ushort[] indices = new ushort[model.Meshes[0].MeshParts[0].IndexBuffer.IndexCount];
            model.Meshes[0].MeshParts[0].IndexBuffer.GetData(indices);

            for (int i = 0; i < model.Meshes[0].MeshParts[0].PrimitiveCount; i++)
                result.Add(new HullTriangle(vertices[indices[3 * i]].Position, vertices[indices[3 * i + 1]].Position, vertices[indices[3 * i + 2]].Position));

            return result;
        }

        /// <summary>
        /// Calc volume.
        /// </summary>
        private static float CalcVolume(Plane waterPlane, List<HullTriangle> triangles)
        {
            float output = 0f;
            Vector3 mom = Vector3.Zero;
            Vector3 tetraOrig = GetTetrahedronOrigin(waterPlane, triangles);

            foreach (HullTriangle triangle in triangles)
            {
                Vector3 a = triangle.Vertex1 - tetraOrig;
                Vector3 b = triangle.Vertex2 - tetraOrig;
                Vector3 c = triangle.Vertex3 - tetraOrig;
                float v = Math.Abs(Vector3.Dot(a, Vector3.Cross(b, c)) / 6f);
                output += v;
            }

            return output;
        }

        private static Vector3 GetTetrahedronOrigin(Plane waterPlane, List<HullTriangle> triangles)
        {
            //IL_000b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            int count = 0;
            Vector3 tetraOrig = Vector3.Zero;

            foreach (HullTriangle triangle in triangles)
            {
                if (Math.Abs(waterPlane.DotCoordinate(triangle.Vertex1)) < 0.001f)
                {
                    tetraOrig += triangle.Vertex1;
                    count++;
                }
                if (Math.Abs(waterPlane.DotCoordinate(triangle.Vertex2)) < 0.001f)
                {
                    tetraOrig += triangle.Vertex2;
                    count++;
                }
                if (Math.Abs(waterPlane.DotCoordinate(triangle.Vertex3)) < 0.001f)
                {
                    tetraOrig += triangle.Vertex3;
                    count++;
                }
            }
                        
            if (count > 0)
                tetraOrig *= 1.0f / count;
            
            return tetraOrig;
        }

        /// <summary>
        /// Update physics.
        /// </summary>
        internal void UpdatePhysics(GameTime time, RigidBody rigidBody)
        {
            Plane waterPlane = GetWaterPlane(rigidBody);


            ClipTriangles(waterPlane);

            float currentVolume = CalcVolume(waterPlane, _clippedTriangles);

            Vector3 force = Vector3.Up * 997f * 9.8f * currentVolume;
            rigidBody.AddForce(new JVector(force.X, force.Y, force.Z), new JVector(VolumeCenter.X, VolumeCenter.Y, VolumeCenter.Z));

            CalcHullResistance(rigidBody);
        }

        /// <summary>
        /// Get water plane.
        /// </summary>
        private Plane GetWaterPlane(RigidBody rigidBody)
        {
            Matrix worldMatrix = Matrix.CreateTranslation(rigidBody.position.X, rigidBody.position.Y, rigidBody.position.Z);

            Vector3 waterBodyPos = Vector3.Transform(Vector3.Zero, Matrix.Invert(worldMatrix));
            Vector3 waterBodyNorm = Vector3.Up;

            return new Plane(waterBodyNorm, 0f - Vector3.Dot(waterBodyPos, waterBodyNorm));
        }

        private void ClipTriangles(Plane waterPlane)
        {
            _clippedTriangles.Clear();

            foreach (HullTriangle triangle in _hullTriangles)
                ClipTriangle(waterPlane, triangle, _clippedTriangles);            
        }

        public static void ClipTriangle(Plane plane, HullTriangle tri, List<HullTriangle> clippedTriangles)
        {
            bool pout0 = plane.DotCoordinate(tri.Vertex1) <= 0f;
            bool pout1 = plane.DotCoordinate(tri.Vertex2) <= 0f;
            bool pout2 = plane.DotCoordinate(tri.Vertex3) <= 0f;
            if (pout0 && pout1 && pout2)
            {
                clippedTriangles.Add(tri);
                return;
            }
            List<Vector3> outputVerts = new List<Vector3>();
            if (pout0)
            {
                outputVerts.Add(tri.Vertex1);
            }
            if (pout0 != pout1)
            {
                Nullable<float> intersect6 = new Ray(tri.Vertex1, tri.Vertex2 - tri.Vertex1).Intersects(plane);
                if (intersect6.HasValue)
                {
                    outputVerts.Add(tri.Vertex1 + (tri.Vertex2 - tri.Vertex1) * intersect6.Value);
                }
            }
            if (pout1)
            {
                outputVerts.Add(tri.Vertex2);
            }
            if (pout1 != pout2)
            {
                Nullable<float> intersect5 = new Ray(tri.Vertex2, tri.Vertex3 - tri.Vertex2).Intersects(plane);
                if (intersect5.HasValue)
                {
                    outputVerts.Add(tri.Vertex2 + (tri.Vertex3 - tri.Vertex2) * intersect5.Value);
                }
            }
            if (pout2)
            {
                outputVerts.Add(tri.Vertex3);
            }
            if (pout2 != pout0)
            {
                Nullable<float> intersect4 = new Ray(tri.Vertex3, tri.Vertex1 - tri.Vertex3).Intersects(plane);
                if (intersect4.HasValue)
                {
                    outputVerts.Add(tri.Vertex3 + (tri.Vertex1 - tri.Vertex3) * intersect4.Value);
                }
            }
            if (outputVerts.Count == 2)
            {
                outputVerts.Clear();
                if (pout0)
                {
                    outputVerts.Add(tri.Vertex1);
                }
                if (pout0 != pout1)
                {
                    Nullable<float> intersect3 = new Ray(tri.Vertex1, tri.Vertex2 - tri.Vertex1).Intersects(plane);
                    if (intersect3.HasValue)
                    {
                        outputVerts.Add(tri.Vertex1 + (tri.Vertex2 - tri.Vertex1) * intersect3.Value);
                    }
                }
                if (pout1)
                {
                    outputVerts.Add(tri.Vertex2);
                }
                if (pout1 != pout2)
                {
                    Nullable<float> intersect2 = new Ray(tri.Vertex2, tri.Vertex3 - tri.Vertex2).Intersects(plane);
                    if (intersect2.HasValue)
                    {
                        outputVerts.Add(tri.Vertex2 + (tri.Vertex3 - tri.Vertex2) * intersect2.Value);
                    }
                }
                if (pout2)
                {
                    outputVerts.Add(tri.Vertex3);
                }
                if (pout2 != pout0)
                {
                    Nullable<float> intersect = new Ray(tri.Vertex3, tri.Vertex1 - tri.Vertex3).Intersects(plane);
                    if (intersect.HasValue)
                    {
                        outputVerts.Add(tri.Vertex3 + (tri.Vertex1 - tri.Vertex3) * intersect.Value);
                    }
                }
            }
            if (outputVerts.Count > 0)
            {
                clippedTriangles.Add(new HullTriangle(outputVerts[0], outputVerts[1], outputVerts[2]));
                if (outputVerts.Count == 4)
                {
                    clippedTriangles.Add(new HullTriangle(outputVerts[0], outputVerts[2], outputVerts[3]));
                }
            }
        }

        private void CalcHullResistance(RigidBody rigidBody)
        {
            switch (ResistanceType)
            {
                case ResistanceType.Damping:
                    CalcDampingResistance(rigidBody);
                    break;
                case ResistanceType.Simple:
                    CalcSimpleResistance(rigidBody);
                    break;
                case ResistanceType.Complex:
                    CalcComplexResistance(rigidBody);
                    break;
            }
            switch (AirResistanceType)
            {
                case AirResistanceType.None:
                    break;
                case AirResistanceType.Damping:
                    CalcDampingAirResistance(rigidBody);
                    break;
                case AirResistanceType.Simple:
                    CalcSimpleAirResistance(rigidBody);
                    break;
            }
        }

        protected void CalcDampingAirResistance(RigidBody rigidBody)
        {
            //Vector3 f = ((ResistanceType == ResistanceType.Damping) ? ResistanceFactor : Vector3.One);
            //f = Vector3.Lerp(AirResistanceFactor, f, CurrentVolume / TotalVolume);
            //rigidBody.AngularVelocity = new JVector(rigidBody.AngularVelocity.X * f.X, rigidBody.AngularVelocity.Y * f.Y, rigidBody.AngularVelocity.Z * f.Z);
            //rigidBody.LinearVelocity = new JVector(rigidBody.LinearVelocity.X * f.X, rigidBody.LinearVelocity.Y * f.Y, rigidBody.LinearVelocity.Z * f.Z);
        }

        protected void CalcSimpleAirResistance(RigidBody rigidBody)
        {
        }

        protected void CalcDampingResistance(RigidBody rigidBody)
        {
        }

        protected void CalcSimpleResistance(RigidBody rigidBody)
        {
            if (_clippedTriangles.Count > 0)
            {
                Vector3 min = _clippedTriangles[0].Vertex1;
                Vector3 max = min;
                for (int i = _clippedTriangles.Count - 1; i >= 0; i--)
                {
                    min.X = Math.Min(Math.Min(Math.Min(min.X, _clippedTriangles[i].Vertex1.X), _clippedTriangles[i].Vertex2.X), _clippedTriangles[i].Vertex3.X);
                    min.Y = Math.Min(Math.Min(Math.Min(min.Y, _clippedTriangles[i].Vertex1.Y), _clippedTriangles[i].Vertex2.Y), _clippedTriangles[i].Vertex3.Y);
                    min.Z = Math.Min(Math.Min(Math.Min(min.Z, _clippedTriangles[i].Vertex1.Z), _clippedTriangles[i].Vertex2.Z), _clippedTriangles[i].Vertex3.Z);
                    max.X = Math.Max(Math.Max(Math.Max(max.X, _clippedTriangles[i].Vertex1.X), _clippedTriangles[i].Vertex2.X), _clippedTriangles[i].Vertex3.X);
                    max.Y = Math.Max(Math.Max(Math.Max(max.Y, _clippedTriangles[i].Vertex1.Y), _clippedTriangles[i].Vertex2.Y), _clippedTriangles[i].Vertex3.Y);
                    max.Z = Math.Max(Math.Max(Math.Max(max.Z, _clippedTriangles[i].Vertex1.Z), _clippedTriangles[i].Vertex2.Z), _clippedTriangles[i].Vertex3.Z);
                }
                Vector3 rC = Vector3.Lerp(min, max, 0.5f);
                Vector3 rBB = max - min;
                Vector3 v = new Vector3(0f - rigidBody.LinearVelocity.X, 0f - rigidBody.LinearVelocity.Y, 0f - rigidBody.LinearVelocity.Z);
                if (v != Vector3.Zero)
                {
                    JVector f = new JVector(rBB.Y * rBB.Z * ResistanceFactor.X * 498.5f * v.X * Math.Abs(v.X), rBB.Z * rBB.X * ResistanceFactor.Y * 498.5f * v.Y * Math.Abs(v.Y), rBB.X * rBB.Y * ResistanceFactor.Z * 498.5f * v.Z * Math.Abs(v.Z));
                    rigidBody.AddForce(f, new JVector(rC.X, rC.Y, rC.Z));
                }
            }
        }

        protected void CalcComplexResistance(RigidBody rigidBody)
        {
            //CurrentWaterSurface = 0f;
            //if (_clippedTriangles.get_Count() > 0)
            //{
            //    Vector3 min = new Vector3(3.40282347E+38f);
            //    Vector3 max = new Vector3(-3.40282347E+38f);
            //    for (int i = _clippedTriangles.get_Count() - 1; i >= 0; i--)
            //    {
            //        min.X = Math.Min(Math.Min(Math.Min(min.X, _clippedTriangles.get_Item(i).Vertex1.X), _clippedTriangles.get_Item(i).Vertex2.X), _clippedTriangles.get_Item(i).Vertex3.X);
            //        min.Y = Math.Min(Math.Min(Math.Min(min.Y, _clippedTriangles.get_Item(i).Vertex1.Y), _clippedTriangles.get_Item(i).Vertex2.Y), _clippedTriangles.get_Item(i).Vertex3.Y);
            //        min.Z = Math.Min(Math.Min(Math.Min(min.Z, _clippedTriangles.get_Item(i).Vertex1.Z), _clippedTriangles.get_Item(i).Vertex2.Z), _clippedTriangles.get_Item(i).Vertex3.Z);
            //        max.X = Math.Max(Math.Max(Math.Max(max.X, _clippedTriangles.get_Item(i).Vertex1.X), _clippedTriangles.get_Item(i).Vertex2.X), _clippedTriangles.get_Item(i).Vertex3.X);
            //        max.Y = Math.Max(Math.Max(Math.Max(max.Y, _clippedTriangles.get_Item(i).Vertex1.Y), _clippedTriangles.get_Item(i).Vertex2.Y), _clippedTriangles.get_Item(i).Vertex3.Y);
            //        max.Z = Math.Max(Math.Max(Math.Max(max.Z, _clippedTriangles.get_Item(i).Vertex1.Z), _clippedTriangles.get_Item(i).Vertex2.Z), _clippedTriangles.get_Item(i).Vertex3.Z);
            //    }
            //    FResistanceCenter = (min + max) / 2f;
            //    FResistanceBB = max - min;
            //    float factor = MathHelper.Clamp(FResistanceBB.X * FResistanceBB.Y * FResistanceBB.Z / (BoundingBox.X * BoundingBox.Y * BoundingBox.Z), 0.5f, 1f);
            //}
        }
    }
}
