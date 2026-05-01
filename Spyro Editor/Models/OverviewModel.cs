using Spyro_Editor.Data;
using Spyro_Editor.Data.Level;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Spyro_Editor.Models
{
    public class OverviewModel : INotifyPropertyChanged
    {
        public int PartCount
        {
            get;
            set { field = value; OnPropertyChanged(); }
        }
        public int TextureCount
        {
            get;
            set { field = value; OnPropertyChanged(); }
        }
        public string? Title
        {
            get;
            set { field = value; OnPropertyChanged(); }
        }
        public string? Type
        {
            get;
            set { field = value; OnPropertyChanged(); }
        }
        public string? DetectedLevel
        {
            get;
            set { field = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public async Task Load(Subfile subfile)
        {
            Title = $"Subfile {subfile.Id}";
            Type = subfile.Type.ToString();
            if (subfile.Type == Constants.SubfileType.Level)
            {
                Level level = new Level();
                using (var stream = await subfile.GetTempFileStream())
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        level.Read(reader);
                    }
                }
                PartCount = level.Ground!.Parts.Length;
                TextureCount = level.Textures!.Length;
                DetectedLevel = subfile.DisplayName.Split("- ")[1];
            }
            else
            {
                DetectedLevel = "(None)";
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
