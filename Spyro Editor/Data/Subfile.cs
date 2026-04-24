using Spyro_Editor.Constants;
using Spyro_Editor.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Spyro_Editor.Data
{
    public class Subfile : IBinaryObject
    {
        public byte Id;
        public byte WADId;
        public uint Offset;
        public uint Size;
        public string DisplayName;
        public SubfileType Type;

        public Subfile(Game game, byte wadId, byte id, uint offset, uint size)
        {
            Id = id;
            WADId = wadId;
            Offset = offset;
            Size = size;
            DisplayName = $"{Id} - {GetDisplayName(game)}";
            Type = GetType(game);
        }

        public void Read(BinaryReader reader)
        {
            throw new System.NotImplementedException();
        }

        private string GetDisplayName(Game game)
        {
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
                    return "UNKNOWN GAME";
            }
            if (names.TryGetValue(Id, out string? name))
            {
                return name;
            }
            else
            {
                return $"0x{Offset.ToString("X")}";
            }
        }

        private SubfileType GetType(Game game)
        {
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
