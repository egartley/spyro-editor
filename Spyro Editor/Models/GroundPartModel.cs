using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;
using SharpDX;
using Spyro_Editor.Constants;
using Spyro_Editor.Data.Level;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spyro_Editor.Models
{
    public class GroundPartModel
    {
        public int Id;
        public Vector3 Centroid;
        public ObservableCollection<GroundPartMeshModel> LowMeshes = new();
        public ObservableCollection<GroundPartMeshModel> HighMeshes = new();
        private Vector3Collection LowVertices = new Vector3Collection();
        private IntCollection LowIndices = new IntCollection();
        private Color4Collection LowColors = new Color4Collection();
        private Dictionary<byte, Vector3Collection> HighVertices = new();
        private Dictionary<byte, IntCollection> HighIndices = new();
        private Dictionary<byte, Color4Collection> HighColors = new();
        private Dictionary<byte, Vector2Collection> HighUVs = new();
        private static byte[] TL = [0, 1];
        private static byte[] TR = [1, 1];
        private static byte[] BL = [0, 0];
        private static byte[] BR = [1, 0];
        private static byte[][] UVPerms = [[0, 1, 2, 3], [3, 0, 1, 2], [2, 3, 0, 1], [1, 2, 3, 0]];

        public GroundPartModel(Part part, int id, Texture[] textures, DiffuseMaterialCollection materials, Game game)
        {
            Id = id;
            foreach (int[] poly in part.LowPolys)
            {
                if (poly[0] == poly[1])
                {
                    PushLowTriangle(poly[1], poly[2], poly[3], poly[5], poly[6], poly[7], part.LowVertices, part.LowColors);
                }
                else if (poly[1] == poly[2])
                {
                    PushLowTriangle(poly[0], poly[2], poly[3], poly[4], poly[6], poly[7], part.LowVertices, part.LowColors);
                }
                else if (poly[2] == poly[3])
                {
                    PushLowTriangle(poly[0], poly[1], poly[3], poly[4], poly[5], poly[7], part.LowVertices, part.LowColors);
                }
                else if (poly[3] == poly[0])
                {
                    PushLowTriangle(poly[0], poly[1], poly[2], poly[4], poly[5], poly[6], part.LowVertices, part.LowColors);
                }
                else
                {
                    PushLowTriangle(poly[1], poly[0], poly[2], poly[5], poly[4], poly[6], part.LowVertices, part.LowColors);
                    PushLowTriangle(poly[1], poly[2], poly[3], poly[5], poly[6], poly[7], part.LowVertices, part.LowColors);
                }
            }
            LowMeshes.Add(new GroundPartMeshModel(new VertColorMaterial(), new MeshGeometry3D()
            {
                Positions = LowVertices,
                Colors = LowColors,
                Indices = LowIndices
            }));
            foreach (HighPolygon poly in part.HighPolys)
            {
                byte textureIndex = (byte)(game == Game.Spyro1 ? (poly.T & 127) : (poly.TT & 127));
                if (textureIndex < 0 || textureIndex >= textures.Length)
                {
                    continue;
                }
                bool isTriangle = poly.V[0] == poly.V[1];
                bool isInverse = game == Game.Spyro1 ? ((poly.II & 4) == 0) : false;
                byte[] a = TL;
                byte[] b = TR;
                byte[] c = BR;
                byte[] d = BL;
                if (game == Game.Spyro1)
                {
                    if (isTriangle)
                    {
                        byte[][] uvs = [TL, TR, BR, BL];
                        byte[] perm = UVPerms[poly.R & 3];
                        a = uvs[perm[0]];
                        b = uvs[perm[1]];
                        c = uvs[perm[2]];
                        d = uvs[perm[3]];
                    }
                }
                else
                {
                    if (isTriangle)
                    {
                        byte rr = (byte)((poly.II >> 4) & 3);
                        byte rot = (byte)((textures[textureIndex].Header.Rotation - rr) & 3);
                        byte[][] seq = [a, b, c, d];
                        a = seq[rot & 3];
                        b = seq[(rot + 1) & 3];
                        c = seq[(rot + 2) & 3];
                        d = seq[(rot + 3) & 3];
                    }
                }
                if (isTriangle)
                {
                    if (isInverse)
                    {
                        PushHighTriangle(textureIndex, poly.V[3], poly.V[2], poly.V[1], poly.C[3], poly.C[2], poly.C[1], d, c, a, part.HighVertices, part.HighColors);
                    }
                    else
                    {
                        PushHighTriangle(textureIndex, poly.V[1], poly.V[2], poly.V[3], poly.C[1], poly.C[2], poly.C[3], a, c, d, part.HighVertices, part.HighColors);
                    }
                }
                else
                {
                    PushHighTriangle(textureIndex, poly.V[0], poly.V[1], poly.V[2], poly.C[0], poly.C[1], poly.C[2], a, b, c, part.HighVertices, part.HighColors);
                    PushHighTriangle(textureIndex, poly.V[0], poly.V[2], poly.V[3], poly.C[0], poly.C[2], poly.C[3], a, c, d, part.HighVertices, part.HighColors);
                }
            }
            foreach (byte key in HighVertices.Keys)
            {
                HighMeshes.Add(new GroundPartMeshModel(materials[key],
                    new MeshGeometry3D()
                    {
                        Positions = HighVertices.Get(key),
                        Colors = HighColors.Get(key),
                        Indices = HighIndices.Get(key),
                        TextureCoordinates = HighUVs.Get(key)
                    }
                ));
            }
            Vector3Collection allVertices = new Vector3Collection();
            foreach (Vector3Collection vertices in HighVertices.Values) {
                allVertices.AddAll(vertices);
            }
            Centroid = allVertices.GetCentroid();
        }

        private void PushLowTriangle(int v1, int v2, int v3, int c1, int c2, int c3, int[][] vertices, byte[][] colors)
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

        private void PushHighTriangle(byte textureIndex, int v1, int v2, int v3, int c1, int c2, int c3, byte[] uv1, byte[] uv2, byte[] uv3, int[][] vertices, byte[][] colors)
        {
            int[] v = [v1, v2, v3];
            int[] c = [c1, c2, c3];
            byte[][] u = [uv1, uv2, uv3];
            for (int i = 0; i < 3; i++)
            {
                float r = colors[c[i]][0] / 255.0f;
                float g = colors[c[i]][1] / 255.0f;
                float b = colors[c[i]][2] / 255.0f;
                Vector3 vertex = new Vector3(vertices[v[i]][0], vertices[v[i]][1], vertices[v[i]][2]);
                Color4 color = new Color4(r, g, b, 1.0f);
                int index = HighIndices.ContainsKey(textureIndex) ? HighIndices.Get(textureIndex).Count : 0;
                Vector2 uv = new Vector2(u[i][0], u[i][1]);

                if (HighVertices.ContainsKey(textureIndex))
                {
                    HighVertices.Get(textureIndex).Add(vertex);
                }
                else
                {
                    var vc = new Vector3Collection();
                    vc.Add(vertex);
                    HighVertices.Add(textureIndex, vc);
                }

                if (HighColors.ContainsKey(textureIndex))
                {
                    HighColors.Get(textureIndex).Add(color);
                }
                else
                {
                    var cc = new Color4Collection();
                    cc.Add(color);
                    HighColors.Add(textureIndex, cc);
                }

                if (HighIndices.ContainsKey(textureIndex))
                {
                    HighIndices.Get(textureIndex).Add(index);
                }
                else
                {
                    var ic = new IntCollection();
                    ic.Add(index);
                    HighIndices.Add(textureIndex, ic);
                }

                if (HighUVs.ContainsKey(textureIndex))
                {
                    HighUVs.Get(textureIndex).Add(uv);
                }
                else
                {
                    var uc = new Vector2Collection();
                    uc.Add(uv);
                    HighUVs.Add(textureIndex, uc);
                }
            }
        }
    }
}
