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
        public Game Game;
        public Level.Level? Level;
        public SubfileType Type;
        private static readonly string TempFolderName = $"instance{Environment.ProcessId}";
        private string TempFileName;

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
            StorageFolder tempFolder = await GetTempFolder();
            StorageFile tempFile = await tempFolder.CreateFileAsync(TempFileName, CreationCollisionOption.ReplaceExisting);
            byte[] buffer = await GetBuffer(true, wadPath);
            await FileIO.WriteBytesAsync(tempFile, buffer);
        }

        public async Task DeleteTemp()
        {
            StorageFolder tempFolder = await GetTempFolder();
            IStorageItem? item = await tempFolder.TryGetItemAsync(TempFileName);
            if (item is not null)
            {
                await item.DeleteAsync();
            }
        }

        public static async Task DeleteTempFolder()
        {
            IStorageItem? item = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(TempFolderName);
            if (item is not null)
            {
                await item.DeleteAsync();
            }
        }

        public async Task LoadLevel()
        {
            byte[] data = await GetBuffer(false);
            Level = await Task.Run(() =>
            {
                Level.Level level = new Level.Level();
                using (var reader = new BinaryReader(new MemoryStream(data)))
                {
                    level.Read(reader, Game);
                }
                return level;
            });
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
                using (var stream = File.Open(wadPath, FileMode.Open, FileAccess.Read, FileShare.Read))
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

        private async Task<StorageFile> GetTempFile()
        {
            StorageFolder tempFolder = await GetTempFolder();
            return await tempFolder.GetFileAsync(TempFileName);
        }

        private static async Task<StorageFolder> GetTempFolder()
        {
            return await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(TempFolderName, CreationCollisionOption.OpenIfExists);
        }

        private string GetDisplayName()
        {
            string defaultName = $"0x{Offset.ToString("X")}";
            Dictionary<short, string> names;
            switch (Game)
            {
                case Game.Spyro1:
                    names = SubfileNames.Spyro1_NTSC;
                    break;
                case Game.Spyro2:
                    names = SubfileNames.Spyro2_NTSC;
                    break;
                case Game.Spyro3:
                    names = SubfileNames.Spyro3_NTSC_1_1;
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
