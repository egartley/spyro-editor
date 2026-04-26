using Spyro_Editor.Constants;
using Spyro_Editor.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Spyro_Editor.Data
{
    public class Subfile : IBinaryObject
    {
        public byte Id;
        public uint Offset;
        public uint Size;
        public string DisplayName;
        public SubfileType Type;

        public Subfile(Game game, byte id, uint offset, uint size)
        {
            Id = id;
            Offset = offset;
            Size = size;
            DisplayName = $"{Id} - {GetDisplayName(game)}";
            Type = GetType(game);
        }

        public void Read(BinaryReader reader)
        {
            throw new System.NotImplementedException();
        }

        public async Task<byte[]> GetBuffer(string wadPath)
        {
            byte[] buffer = new byte[Size];
            using (var stream = File.Open(wadPath, FileMode.Open))
            {
                stream.Seek(Offset, SeekOrigin.Begin);
                await stream.ReadExactlyAsync(buffer, 0, (int)Size);
            }
            return buffer;
        }

        private string GetDisplayName(Game game)
        {
            string defaultName = $"0x{Offset.ToString("X")}";
            Dictionary<byte, string> names;
            switch (game)
            {
                case Game.Spyro1:
                    names = SubfileNames.Spyro1;
                    break;
                case Game.Spyro2:
                    names = SubfileNames.Spyro2;
                    break;
                case Game.Spyro3:
                    names = SubfileNames.Spyro3;
                    break;
                default:
                    return defaultName;
            }
            if (names.TryGetValue(Id, out string? name))
            {
                return name;
            }
            else
            {
                return defaultName;
            }
        }

        private SubfileType GetType(Game game)
        {
            /*
             * https://github.com/egartley/noclip.website/blob/main/src/Spyro/tools/extractor.py
            subfile_type_map = [
                {"level": range(10, 79, 2), "cutscene": range(3, 7, 1), "starring": range(82, 102, 1)},
                {"level": range(15, 72, 2), "cutscene": range(73, 96, 2), "starring": range(187, 197, 1)},
                {"level": range(97, 170, 2), "cutscene": range(6, 67, 3), "starring": range(183, 195, 1)}
            ]
             */
            switch (game)
            {
                case Game.Spyro1:
                    if (11 <= Id && 79 >= Id)
                    {
                        if (Id % 2 == 0)
                        {
                            return SubfileType.Overlay;
                        }
                        return SubfileType.Level;
                    }
                    else if (4 <= Id && 7 >= Id)
                    {
                        return SubfileType.Cutscene;
                    }
                    else if (83 <= Id && 102 >= Id)
                    {
                        return SubfileType.Flyover;
                    }
                    break;
                case Game.Spyro2:
                    if (16 <= Id && 72 >= Id)
                    {
                        if (Id % 2 == 0)
                        {
                            return SubfileType.Level;
                        }
                        return SubfileType.Overlay;
                    }
                    else if (74 <= Id && 96 >= Id)
                    {
                        if (Id % 2 == 0)
                        {
                            return SubfileType.Cutscene;
                        }
                        return SubfileType.Other;
                    }
                    else if (188 <= Id && 197 >= Id)
                    {
                        return SubfileType.Flyover;
                    }
                    break;
                case Game.Spyro3:
                    if (98 <= Id && 170 >= Id)
                    {
                        if (Id % 2 == 0)
                        {
                            return SubfileType.Level;
                        }
                        return SubfileType.Overlay;
                    }
                    else if (7 <= Id && 67 >= Id)
                    {
                        if ((Id - 1) % 3 == 0)
                        {
                            return SubfileType.Cutscene;
                        }
                        return SubfileType.Other;
                    }
                    else if (184 <= Id && 195 >= Id)
                    {
                        return SubfileType.Flyover;
                    }
                    break;
            }
            return SubfileType.Other;
        }
    }
}
