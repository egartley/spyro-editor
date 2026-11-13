using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;

namespace Spyro_Editor.Models
{
    public class GroundPartMeshModel
    {
        public Material Material;
        public MeshGeometry3D Mesh;

        public GroundPartMeshModel(Material material, MeshGeometry3D mesh)
        {
            Material = material;
            Mesh = mesh;
        }
    }
}
