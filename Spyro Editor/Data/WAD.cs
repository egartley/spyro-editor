using Spyro_Editor.Constants;
using Spyro_Editor.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Spyro_Editor.Data
{
    public class WAD : IBinaryObject
    {
        public string DisplayName;
        public List<Subfile> Subfiles = [];
        private string Path;

        public WAD(BinaryReader reader, string path)
        {
            Path = path;
            reader.BaseStream.Seek(0x100, SeekOrigin.Begin);
            uint magic = reader.ReadUInt32();
            switch (magic)
            {
                case (uint)WADSignature.Spyro1_NTSC:
                    DisplayName = "Spyro the Dragon (NTSC)";
                    break;
                case (uint)WADSignature.Spyro2_NTSC:
                    DisplayName = "Spyro 2: Ripto's Rage! (NTSC)";
                    break;
                case (uint)WADSignature.Spyro3_NTSC:
                    DisplayName = "Spyro: Year of the Dragon (NTSC)";
                    break;
                default:
                    DisplayName = "UNKNOWN WAD";
                    break;
            }
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            Read(reader);
        }

        public void Read(BinaryReader reader)
        {
            byte strikes = 0;
            while (strikes < 3)
            {
                uint offset = reader.ReadUInt32();
                uint size = reader.ReadUInt32();
                if (offset == 0 && size == 0)
                {
                    strikes++;
                }
                else
                {
                    strikes = 0;
                    if (size > 0)
                    {
                        Subfiles.Add(new Subfile(offset, size));
                    }
                }
            }
        }
    }

    public class Subfile
    {
        public string DisplayName;

        public Subfile(uint offset, uint size)
        {
            DisplayName = $"0x{offset.ToString("X")} ({size})";
        }
    }
}
