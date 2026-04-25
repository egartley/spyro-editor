using Spyro_Editor.Constants;
using Spyro_Editor.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Spyro_Editor.Data
{
    public class WAD : IBinaryObject
    {
        public string Path;
        public string DisplayName;
        public List<Subfile> Subfiles = [];
        private Game Game;

        public WAD(BinaryReader reader, string path)
        {
            Path = path;
            reader.BaseStream.Seek(0x100, SeekOrigin.Begin);
            uint magic = reader.ReadUInt32();
            switch (magic)
            {
                case (uint)WADSignature.Spyro1_NTSC:
                    Game = Game.Spyro1;
                    DisplayName = "Spyro the Dragon (NTSC)";
                    break;
                case (uint)WADSignature.Spyro2_NTSC:
                    Game = Game.Spyro2;
                    DisplayName = "Spyro 2: Ripto's Rage! (NTSC)";
                    break;
                case (uint)WADSignature.Spyro3_NTSC:
                    Game = Game.Spyro3;
                    DisplayName = "Spyro: Year of the Dragon (NTSC)";
                    break;
                default:
                    DisplayName = "(Unknown WAD)";
                    break;
            }
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            Read(reader);
        }

        public void Read(BinaryReader reader)
        {
            byte index = 0;
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
                        Subfiles.Add(new Subfile(Game, (byte)(index + 1), offset, size));
                    }
                }
                index++;
            }
        }
    }
}
