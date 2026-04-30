using HelixToolkit.SharpDX.Core;
using SharpDX;
using Spyro_Editor.Interfaces;
using Spyro_Editor.Utils;
using System.IO;

namespace Spyro_Editor.Data.Level
{
    public class LevelData : IBinaryObject
    {
        public Vector3Collection Vertices;
        public IntCollection Indices;
        public Color4Collection Colors;
        private byte[][]? Textures;
        private Ground? Ground;

        public LevelData()
        {
            Vertices = new Vector3Collection();
            Indices = new IntCollection();
            Colors = new Color4Collection();
        }

        public void Build()
        {
            if (Ground is not null)
            {
                foreach (Part part in Ground.Parts)
                {
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
                }
            }
        }

        public void Read(BinaryReader reader)
        {
            uint vramOffset = reader.ReadUInt32();

            reader.BaseStream.Seek(8, SeekOrigin.Begin);
            uint textureOffset = reader.ReadUInt32();
            reader.BaseStream.Seek(textureOffset, SeekOrigin.Begin);
            uint textureSize = reader.ReadUInt32();
            reader.BaseStream.Seek((int)(textureSize - 4), SeekOrigin.Current);

            long groundOffset = reader.BaseStream.Position;
            uint groundSize = reader.ReadUInt32();
            reader.BaseStream.Seek((int)(groundSize - 4), SeekOrigin.Current);

            uint skySize = reader.ReadUInt32();
            reader.BaseStream.Seek(-4, SeekOrigin.Current);
            long skyBegin = reader.BaseStream.Position;
            uint skySizeBase = skySize;
            reader.BaseStream.Seek(skySize, SeekOrigin.Current);
            skySize = reader.ReadUInt32();
            if (skySize > 3)
            {
                reader.BaseStream.Seek(skySize - 4, SeekOrigin.Current);
                skySize = reader.ReadUInt32();
                reader.BaseStream.Seek(skySize - 4, SeekOrigin.Current);
                skySize = reader.ReadUInt32();
            }
            else
            {
                reader.BaseStream.Seek(skyBegin, SeekOrigin.Begin);
                skySize = skySizeBase;
            }
            byte[] sky = reader.ReadBytes((int)skySize);

            reader.BaseStream.Seek(24, SeekOrigin.Begin);
            uint subfile4Offset = reader.ReadUInt32();
            uint subfile4Size = reader.ReadUInt32();
            reader.BaseStream.Seek(subfile4Offset, SeekOrigin.Begin);
            byte[] subfile4 = reader.ReadBytes((int)subfile4Size);

            Parse(reader, vramOffset, textureOffset, groundOffset);
        }

        private void Parse(BinaryReader reader, uint vramOffset, uint textureOffset, long groundOffset)
        {
            reader.BaseStream.Seek(vramOffset, SeekOrigin.Begin);
            VRAM vram = new VRAM();
            vram.Read(reader);

            reader.BaseStream.Seek(textureOffset, SeekOrigin.Begin);
            TextureTable textureTable = new TextureTable();
            textureTable.Read(reader);

            Textures = new byte[textureTable.Headers.Length][];
            for (int i = 0; i < textureTable.Headers.Length; i++)
            {
                byte[] rgba = TextureDecode.DecodeTexture(vram, textureTable.Headers[i]);
                rgba = TextureDecode.RotateTexture(rgba, textureTable.Headers[i]);
                Textures[i] = rgba;
            }

            reader.BaseStream.Seek(groundOffset, SeekOrigin.Begin);
            Ground = new Ground();
            Ground.Read(reader);
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
                Vertices.Add(new Vector3(vertices[vi][0] / 1000.0f, vertices[vi][1] / 1000.0f, vertices[vi][2] / 1000.0f));
                Colors.Add(new Color4(r, g, b, 1.0f));
                int index = Indices.Count;
                Indices.Add(index);
            }
        }
    }
}
