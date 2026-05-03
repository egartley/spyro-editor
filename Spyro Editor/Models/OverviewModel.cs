using Spyro_Editor.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

        public void Load(Subfile subfile)
        {
            Title = $"Subfile {subfile.Id}";
            Type = subfile.Type.ToString();
            if (subfile.Level is not null)
            {
                PartCount = subfile.Level.Ground!.Parts.Length;
                TextureCount = subfile.Level.Textures!.Length;
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
