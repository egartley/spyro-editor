using Spyro_Editor.Constants;
using System.Collections.Generic;

namespace Spyro_Editor.Data
{
    public class Subfile
    {
        public string DisplayName;
        public SubfileType Type;
        private byte Id;
        private uint Offset;
        private uint Size;
        private Game Game;

        public Subfile(Game game, byte id, uint offset, uint size)
        {
            Id = id;
            Offset = offset;
            Size = size;
            Game = game;
            DisplayName = $"{Id} - {GetDisplayName()}";
            Type = GetType();
        }

        private string GetDisplayName()
        {
            Dictionary<byte, string> names;
            switch (Game)
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

        private SubfileType GetType()
        {
            switch (Game)
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
