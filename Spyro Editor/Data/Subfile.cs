using Spyro_Editor.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Spyro_Editor.Data
{
    public class Subfile
    {
        public short Id;
        public uint Offset;
        public uint Size;
        public string DisplayName;
        public Level.Level? Level;
        public SubfileType Type;
        private string TempFileName;
        private Game Game;

        public Subfile(Game game, short id, uint offset, uint size)
        {
            Id = id;
            Game = game;
            Offset = offset;
            Size = size;
            Type = GetSubfileType();
            DisplayName = $"{Id} - {GetDisplayName()}";
            TempFileName = $"sf{Id}.bin";
        }

        public async Task WriteTemp(string wadPath)
        {
            StorageFile tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(TempFileName, CreationCollisionOption.ReplaceExisting);
            byte[] buffer = await GetBuffer(true, wadPath);
            await FileIO.WriteBytesAsync(tempFile, buffer);
        }

        public async Task DeleteTemp()
        {
            StorageFile file = await GetTempFile();
            await file.DeleteAsync();
        }

        public async Task LoadLevel()
        {
            Level = new Level.Level();
            using (var stream = await GetTempFileStream())
            {
                using (var reader = new BinaryReader(stream))
                {
                    Level.Read(reader, Game);
                }
            }
        }

        public async Task<byte[]> GetBuffer(bool readFromWAD, string wadPath = "")
        {
            if (!readFromWAD)
            {
                StorageFile tempFile = await GetTempFile();
                var buffer = await FileIO.ReadBufferAsync(tempFile);
                byte[] data = new byte[buffer.Length];
                using (var reader = DataReader.FromBuffer(buffer))
                {
                    reader.ReadBytes(data);
                }
                return data;
            }
            else if (wadPath.Length > 0)
            {
                byte[] buffer = new byte[Size];
                using (var stream = File.Open(wadPath, FileMode.Open))
                {
                    stream.Seek(Offset, SeekOrigin.Begin);
                    await stream.ReadExactlyAsync(buffer, 0, (int)Size);
                }
                return buffer;
            }
            else
            {
                return Array.Empty<byte>();
            }
        }

        public async Task<Stream> GetTempFileStream()
        {
            StorageFile tempFile = await GetTempFile();
            return await tempFile.OpenStreamForReadAsync();
        }

        private async Task<StorageFile> GetTempFile()
        {
            return await ApplicationData.Current.TemporaryFolder.GetFileAsync(TempFileName);
        }

        private string GetDisplayName()
        {
            string defaultName = $"0x{Offset.ToString("X")}";
            Dictionary<short, string> names;
            switch (Game)
            {
                case Game.Spyro1:
                    names = SubfileNames.Spyro1_NSTC;
                    break;
                case Game.Spyro2:
                    names = SubfileNames.Spyro2_NSTC;
                    break;
                case Game.Spyro3:
                    names = SubfileNames.Spyro3_NSTC_1_1;
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

        private SubfileType GetSubfileType()
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
