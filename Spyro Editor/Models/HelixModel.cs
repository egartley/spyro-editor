using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;
using Spyro_Editor.Data;
using Spyro_Editor.Data.Level;
using System.IO;
using System.Threading.Tasks;

namespace Spyro_Editor.Models
{
    public class HelixModel
    {
        public Material Material;
        public MeshGeometry3D Mesh;

        public HelixModel()
        {
            Mesh = new MeshGeometry3D();
            Material = new VertColorMaterial();
        }

        public async Task Load(Subfile subfile)
        {
            LevelData data = new LevelData();
            using (var stream = await subfile.GetTempFileStream())
            {
                using (var reader = new BinaryReader(stream))
                {
                    data.Read(reader);
                }
            }
            data.Build();
            Mesh = new MeshGeometry3D
            {
                Positions = data.Vertices,
                Indices = data.Indices,
                Colors = data.Colors
            };
        }
    }
}
