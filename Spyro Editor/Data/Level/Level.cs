using Spyro_Editor.Constants;
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
        public MobyInstance[]? MobyInstances;

        public void Read(BinaryReader reader, Game game)
        {
            // gather offsets

            uint vramOffset = reader.ReadUInt32();

            reader.BaseStream.Seek(8, SeekOrigin.Begin);
            uint textureOffset = reader.ReadUInt32();
            reader.BaseStream.Seek(textureOffset, SeekOrigin.Begin);
            uint textureSize = reader.ReadUInt32();
            reader.BaseStream.Seek((int)(textureSize - 4), SeekOrigin.Current);

            long groundOffset = reader.BaseStream.Position;
            uint groundSize = reader.ReadUInt32();
            reader.BaseStream.Seek((int)(groundSize - 4), SeekOrigin.Current);

            //uint skySize = reader.ReadUInt32();
            //reader.BaseStream.Seek(-4, SeekOrigin.Current);
            //long skyBegin = reader.BaseStream.Position;
            //uint skySizeBase = skySize;
            //reader.BaseStream.Seek(skySize, SeekOrigin.Current);
            //skySize = reader.ReadUInt32();
            //if (skySize > 3)
            //{
            //    reader.BaseStream.Seek(skySize - 4, SeekOrigin.Current);
            //    skySize = reader.ReadUInt32();
            //    reader.BaseStream.Seek(skySize - 4, SeekOrigin.Current);
            //    skySize = reader.ReadUInt32();
            //}
            //else
            //{
            //    reader.BaseStream.Seek(skyBegin, SeekOrigin.Begin);
            //    skySize = skySizeBase;
            //}
            //byte[] sky = reader.ReadBytes((int)skySize);

            reader.BaseStream.Seek(24, SeekOrigin.Begin);
            uint subfile4Offset = reader.ReadUInt32();

            // parse sections

            reader.BaseStream.Seek(vramOffset, SeekOrigin.Begin);
            VRAM vram = new VRAM();
            vram.Read(reader, game);
            if (game == Game.Spyro2)
            {
                vram.ApplyFontStripFix();
            }

            reader.BaseStream.Seek(textureOffset, SeekOrigin.Begin);
            TextureTable textureTable = new TextureTable();
            textureTable.Read(reader, game);

            Textures = new Texture[textureTable.Count];
            for (int i = 0; i < textureTable.Count; i++)
            {
                if (game == Game.Spyro1)
                {
                    Textures[i] = new Texture(vram, textureTable.LODHeaders[i], textureTable.MIDHeaders[i], textureTable.CORHeaders[i], textureTable.TNYHeaders[i], textureTable.SPRHeaders[i]);
                }
                else
                {
                    Textures[i] = new Texture(vram, textureTable.LODHeaders[i], textureTable.MIDHeaders[i], textureTable.CORHeaders[i]);
                }
            }

            reader.BaseStream.Seek(groundOffset, SeekOrigin.Begin);
            Ground = new Ground();
            Ground.Read(reader, game);

            reader.BaseStream.Seek(subfile4Offset, SeekOrigin.Begin);
            byte[] sectionIndex = [7, 8, 12];
            byte[] startOffsets = [136, 44, 48];
            reader.BaseStream.Seek(startOffsets[(int)game], SeekOrigin.Current);
            for (int i = 0; i < sectionIndex[(int)game]; i++)
            {
                uint skip = reader.ReadUInt32();
                // skip is from the section start, so go back 4 bytes
                reader.BaseStream.Seek(skip - 4, SeekOrigin.Current);
            }
            // go over the instance section's skip
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            uint instanceCount = reader.ReadUInt32();
            MobyInstances = new MobyInstance[instanceCount];
            for (int i = 0; i < instanceCount; i++)
            {
                long startPos = reader.BaseStream.Position;
                MobyInstance instance = new MobyInstance();
                instance.Read(reader, game);
                MobyInstances[i] = instance;
                reader.BaseStream.Seek(startPos + MobyInstance.SIZE, SeekOrigin.Begin);
            }
        }
    }
}
