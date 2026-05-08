using Spyro_Editor.Constants;
using Spyro_Editor.Interfaces;
using Spyro_Editor.Utils;
using System.IO;

namespace Spyro_Editor.Data.Level
{
    /// <summary>
    /// The "ground" section of a level subfile. Contains both the visible geometry and collision data
    /// </summary>
    /// <seealso cref="Part"/>
    /// <seealso cref="PartHeader"/>
    public class Ground : IBinaryObject
    {
        public Part[] Parts = [];

        public void Read(BinaryReader reader, Game game)
        {
            long startPos = reader.BaseStream.Position;
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            uint partCount = reader.ReadUInt32();

            long[] partOffsets = new long[partCount];
            if (game == Game.Spyro1)
            {
                for (int i = 0; i < partCount; i++)
                {
                    uint offset = reader.ReadUInt32();
                    partOffsets[i] = reader.BaseStream.Position + offset - (4 * i);
                }
            }
            else
            {
                partCount = reader.ReadUInt32();
                partOffsets = new long[partCount];
                for (int i = 0; i < partCount; i++)
                {
                    uint offset = reader.ReadUInt32();
                    partOffsets[i] = startPos + offset + 12;
                }
            }

            Parts = new Part[partCount];
            for (int i = 0; i < partCount; i++)
            {
                reader.BaseStream.Seek(partOffsets[i], SeekOrigin.Begin);
                PartHeader header = new PartHeader();
                header.Read(reader, game);
                Part part = new Part(header);
                part.Read(reader, game);
                Parts[i] = part;
            }
        }
    }

    /// <summary>
    /// A 20-byte header that describes a <see cref="Part"/>
    /// </summary>
    public class PartHeader : IBinaryObject
    {
        public short x;
        public short y;
        public short z;
        public ushort i0;
        public ushort i1;
        public byte LowVertexCount;
        public byte LowColorCount;
        public byte LowPolyCount;
        public byte HighVertexCount;
        public byte HighColorCount;
        public byte HighPolyCount;
        public byte Water;
        public uint Flags;

        public void Read(BinaryReader reader, Game game)
        {
            y = reader.ReadInt16();
            x = reader.ReadInt16();
            i0 = reader.ReadUInt16();
            z = reader.ReadInt16();
            LowVertexCount = reader.ReadByte();
            LowColorCount = reader.ReadByte();
            LowPolyCount = reader.ReadByte();
            i1 = reader.ReadByte();
            HighVertexCount = reader.ReadByte();
            HighColorCount = reader.ReadByte();
            HighPolyCount = reader.ReadByte();
            Water = reader.ReadByte();
            Flags = reader.ReadUInt32();
        }
    }

    /// <summary>
    /// A part of the visible geometry of a level. Contains data for both LODs
    /// </summary>
    public class Part : IBinaryObject
    {
        public int[][] LowVertices = [];
        public int[][] HighVertices = [];
        public byte[][] LowColors = [];
        public byte[][] HighColors = [];
        public int[][] LowPolys = [];
        public byte[][] HighPolys = [];
        private PartHeader Header;

        public Part(PartHeader header)
        {
            Header = header;
        }

        public void Read(BinaryReader reader, Game game)
        {
            LowVertices = new int[Header.LowVertexCount][];
            for (int i = 0; i < Header.LowVertexCount; i++)
            {
                byte[] bytes = reader.ReadBytes(4);
                LowVertices[i] = GeometryDecode.DecodeVertex(bytes, game, Header.x, Header.y, Header.z);
            }

            LowColors = new byte[Header.LowColorCount][];
            for (int i = 0; i < Header.LowColorCount; i++)
            {
                LowColors[i] = reader.ReadBytes(4);
            }

            LowPolys = new int[Header.LowPolyCount][];
            for (int i = 0; i < Header.LowPolyCount; i++)
            {
                byte[] bytes = reader.ReadBytes(8);
                LowPolys[i] = GeometryDecode.DecodeLowPoly(bytes, game);
            }

            HighVertices = new int[Header.HighVertexCount][];
            for (int i = 0; i < Header.HighVertexCount; i++)
            {
                byte[] bytes = reader.ReadBytes(4);
                HighVertices[i] = GeometryDecode.DecodeVertex(bytes, game, Header.x, Header.y, Header.z);
            }

            HighColors = new byte[Header.HighColorCount][];
            for (int i = 0; i < Header.HighColorCount; i++)
            {
                HighColors[i] = reader.ReadBytes(4);
            }

            // idk what this is skipping, but needed to get to the high polys
            reader.BaseStream.Seek(Header.HighColorCount * 4, SeekOrigin.Current);

            HighPolys = new byte[Header.HighPolyCount][];
            for (int i = 0; i < Header.HighPolyCount; i++)
            {
                HighPolys[i] = reader.ReadBytes(16);
            }
        }
    }
}
