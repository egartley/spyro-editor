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
                    DisplayName = "Spyro 2: Ripto's Rage!"; // no region since title is different
                    break;
                case (uint)WADSignature.Spyro1_PAL:
                    Game = Game.Spyro1;
                    DisplayName = "Spyro the Dragon (PAL)";
                    break;
                case (uint)WADSignature.Spyro2_PAL:
                    Game = Game.Spyro2;
                    DisplayName = "Spyro 2: Gateway to Glimmer"; // no region since title is different
                    break;
                case (uint)WADSignature.Spyro3_PAL:
                    Game = Game.Spyro3;
                    DisplayName = "Spyro: Year of the Dragon (PAL)";
                    break;
                default:
                    // 3 nstc requires a different magic since 0x100 is the same between 1.0 and 1.1
                    reader.BaseStream.Seek(0x400, SeekOrigin.Begin);
                    uint magic2 = reader.ReadUInt32();
                    switch (magic2)
                    {
                        case (uint)WADSignature.Spyro3_NTSC_1_0:
                            Game = Game.Spyro3;
                            DisplayName = "Spyro: Year of the Dragon (1.0) (NTSC)";
                            break;
                        case (uint)WADSignature.Spyro3_NTSC_1_1:
                            Game = Game.Spyro3;
                            DisplayName = "Spyro: Year of the Dragon (1.1) (NTSC)";
                            break;
                        default:
                            Game = Game.Spyro1;
                            DisplayName = "(Unknown WAD)";
                            break;
                    }
                    break;
            }
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            Read(reader);
        }

        public void Read(BinaryReader reader)
        {
            short index = 0;
            byte strikes = 0;
            while (strikes < 3)
            {
                uint offset = reader.ReadUInt32();
                uint size = reader.ReadUInt32();
                if (offset == 0 && size == 0)
                {
                    // allow two zero-zero entries, break on third (you're out!)
                    strikes++;
                }
                else
                {
                    strikes = 0;
                    if (size > 0)
                    {
                        Subfiles.Add(new Subfile(Game, (short)(index + 1), offset, size));
                    }
                }
                if (index + 1 == short.MaxValue)
                {
                    // something's very wrong
                    break;
                }
                index++;
            }
        }
    }
}
