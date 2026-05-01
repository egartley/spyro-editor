using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;
using Spyro_Editor.Data;
using Spyro_Editor.Data.Level;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Spyro_Editor.Models
{
    public partial class HelixModel : INotifyPropertyChanged
    {
        public Material Material;
        public MeshGeometry3D? Mesh
        {
            get;
            set { field = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public HelixModel()
        {
            Material = new VertColorMaterial();
        }

        public async Task Load(Subfile subfile)
        {
            Level level = new Level();
            using (var stream = await subfile.GetTempFileStream())
            {
                using (var reader = new BinaryReader(stream))
                {
                    level.Read(reader);
                }
            }
            LevelModel data = new LevelModel(level.Ground!);
            Mesh = new MeshGeometry3D
            {
                Positions = data.LowVertices,
                Indices = data.LowIndices,
                Colors = data.LowColors
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
