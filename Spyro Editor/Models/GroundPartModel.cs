using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;
using SharpDX;
using Spyro_Editor.Data.Level;

namespace Spyro_Editor.Models
{
    public class GroundPartModel
    {
        public int Id;
        public Material Material;
        public MeshGeometry3D? Mesh;
        public Vector3 Centroid;
        private Vector3Collection LowVertices = new Vector3Collection();
        private IntCollection LowIndices = new IntCollection();
        private Color4Collection LowColors = new Color4Collection();

        public GroundPartModel(Part part, int id)
        {
            Id = id;
            Material = new VertColorMaterial();
            foreach (int[] poly in part.LowPolys)
            {
                if (poly[0] == poly[1])
                {
                    PushTriangle(poly[1], poly[2], poly[3], poly[5], poly[6], poly[7], part.LowVertices, part.LowColors);
                }
                else if (poly[1] == poly[2])
                {
                    PushTriangle(poly[0], poly[2], poly[3], poly[4], poly[6], poly[7], part.LowVertices, part.LowColors);
                }
                else if (poly[2] == poly[3])
                {
                    PushTriangle(poly[0], poly[1], poly[3], poly[4], poly[5], poly[7], part.LowVertices, part.LowColors);
                }
                else if (poly[3] == poly[0])
                {
                    PushTriangle(poly[0], poly[1], poly[2], poly[4], poly[5], poly[6], part.LowVertices, part.LowColors);
                }
                else
                {
                    PushTriangle(poly[1], poly[0], poly[2], poly[5], poly[4], poly[6], part.LowVertices, part.LowColors);
                    PushTriangle(poly[1], poly[2], poly[3], poly[5], poly[6], poly[7], part.LowVertices, part.LowColors);
                }
            }
            Mesh = new MeshGeometry3D
            {
                Positions = LowVertices,
                Indices = LowIndices,
                Colors = LowColors
            };
            Centroid = Mesh.Positions.GetCentroid();
        }

        private void PushTriangle(int v1, int v2, int v3, int c1, int c2, int c3, int[][] vertices, byte[][] colors)
        {
            int[] v = [v1, v2, v3];
            int[] c = [c1, c2, c3];
            for (int i = 0; i < 3; i++)
            {
                int vi = v[i];
                int ci = c[i];
                float r = colors[ci][0] / 255.0f;
                float g = colors[ci][1] / 255.0f;
                float b = colors[ci][2] / 255.0f;
                LowVertices.Add(new Vector3(vertices[vi][0], vertices[vi][1], vertices[vi][2]));
                LowColors.Add(new Color4(r, g, b, 1.0f));
                int index = LowIndices.Count;
                LowIndices.Add(index);
            }
        }
    }
}
