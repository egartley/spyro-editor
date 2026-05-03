using Spyro_Editor.Interfaces;
using System.IO;

namespace Spyro_Editor.Data.Level
{
    /// <summary>
    /// Data from a level subfile. Consists of various sections that start with a size/jump
    /// </summary>
    public class Level : IBinaryObject
    {
        public Ground? Ground;
        public Texture[]? Textures;

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

            Textures = new Texture[textureTable.Count];
            for (int i = 0; i < textureTable.Count; i++)
            {
                Textures[i] = new Texture(vram, textureTable.LODHeaders[i], textureTable.MIDHeaders[i],
                    textureTable.SPRHeaders[i], textureTable.CORHeaders[i], textureTable.TNYHeaders[i]);
            }

            reader.BaseStream.Seek(groundOffset, SeekOrigin.Begin);
            Ground = new Ground();
            Ground.Read(reader);
        }
    }
}
