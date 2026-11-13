using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spyro_Editor.Utils
{
    public class MathHelpers
    {
        public static Vector3 MergeCentroids(IEnumerable<Vector3> centroids)
        {
            float averageX = 0.0f, averageY = 0.0f, averageZ = 0.0f;
            foreach (Vector3 c in centroids)
            {
                averageX += c.X;
                averageY += c.Y;
                averageZ += c.Z;
            }
            averageX /= centroids.Count();
            averageY /= centroids.Count();
            averageZ /= centroids.Count();
            return new Vector3(averageX, averageY, averageZ);
        }
    }
}
