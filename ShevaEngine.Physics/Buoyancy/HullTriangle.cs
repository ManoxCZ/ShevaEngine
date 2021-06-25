using System;
using Microsoft.Xna.Framework;

namespace ShevaEngine.Physics
{
    public struct HullTriangle
    {
        public readonly Vector3 Vertex1;
        public readonly Vector3 Vertex2;
        public readonly Vector3 Vertex3;
        public readonly Vector3 Normal;
        public readonly Vector3 Center;
        public readonly float Surface;


        /// <summary>
        /// Constructor.
        /// </summary>
        public HullTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Vertex3 = vertex3;

            Normal = Vector3.Normalize(Vector3.Cross(Vertex2 - Vertex1, Vertex3 - Vertex1));
            Center = (Vertex1 + Vertex2 + Vertex3) / 3f;
            Surface = 0.5f * Vector3.Cross(Vertex2 - Vertex1, Vertex3 - Vertex1).Length();
        }
    }
}