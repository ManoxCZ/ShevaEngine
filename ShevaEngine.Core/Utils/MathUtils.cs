using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ShevaEngine.Core
{
    public class MathUtils
    {
        /// <summary>
        /// Lerp.
        /// </summary>
        public static double Lerp(double a, double b, double t)
        {
            t = Math.Clamp(t, 0, 1);

            return a * (1.0 - t) + b * t;
        }

        /// <summary>
        /// Lerp.
        /// </summary>
        public static int LerpInt32(int a, int b, double t)
        {
            t = Math.Clamp(t, 0, 1);

            return (int)(a * (1.0 - t) + b * t);
        }

        /// <summary>
        /// Intersects.
        /// </summary>		
        public static bool Intersects(IList<Vector2> polygon1, IList<Vector2> polygon2)
        {
            for (int iPol1 = 0; iPol1 < polygon1.Count - 1; iPol1++)
            {
                for (int iPol2 = 0; iPol2 < polygon2.Count - 1; iPol2++)
                {
                    if (Intersects(
                        polygon1[iPol1],
                        polygon1[(iPol1 + 1) % polygon1.Count],
                        polygon2[iPol2],
                        polygon2[(iPol2 + 1) % polygon2.Count], 
                        out Vector2? _))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Intersects.
        /// </summary>		
        public static bool Intersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2? intersection)
        {
            intersection = null;

            Vector2 b = Vector2.Subtract(a2, a1);
            Vector2 d = Vector2.Subtract(b2, b1);

            float bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
            {
                return false;
            }

            Vector2 c = Vector2.Subtract(b1, a1);

            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;

            if (t < 0 || t > 1)
            {
                return false;
            }

            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;

            if (u < 0 || u > 1)
            {
                return false;
            }

            intersection = a1 + Vector2.Multiply(b, t);

            return true;
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// </summary>
        public static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }

            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }

            return radians;
        }
    }
}
