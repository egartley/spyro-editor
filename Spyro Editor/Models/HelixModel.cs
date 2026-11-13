using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;
using SharpDX;
using SharpDX.Direct3D11;
using Spyro_Editor.Constants;
using Spyro_Editor.Data.Level;
using Spyro_Editor.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Spyro_Editor.Models
{
    public partial class HelixModel
    {
        private List<GroundPartModel> AllParts = [];
        public Vector3 Centroid;
        public ObservableCollection<GroundPartModel> VisibleMeshes = new();
        public ObservableCollection<PartListItem> ListItems = new();
        private DiffuseMaterialCollection Materials = new();

        public async Task Load(Level level, Game game)
        {
            VisibleMeshes.Clear();
            AllParts.Clear();
            ListItems.Clear();
            Materials.Clear();
            SamplerStateDescription sampler = SamplerStateDescription.Default();
            sampler.AddressU = TextureAddressMode.Clamp;
            sampler.AddressV = TextureAddressMode.Clamp;
            sampler.Filter = Filter.MinMagPointMipLinear;
            foreach (Texture texture in level.Textures!)
            {
                Materials.Add(new DiffuseMaterial()
                {
                    DiffuseMap = new TextureModel(texture.COR, Texture.COR_SIZE, Texture.COR_SIZE),
                    DiffuseMapSampler = sampler,
                    VertexColorBlendingFactor = 0.3
                });
            }
            for (int i = 0; i < level.Ground!.Parts.Length; i++)
            {
                GroundPartModel m = new GroundPartModel(level.Ground!.Parts[i], i, level.Textures!, Materials, game);
                AllParts.Add(m);
                VisibleMeshes.Add(m);
                ListItems.Add(new PartListItem() { Id = i, IsVisible = true });
            }
            Centroid = MathHelpers.MergeCentroids(AllParts.Select(p => p.Centroid));
        }

        public void UpdateMeshVisibility(int id, bool isVisible)
        {
            bool partAlreadyVisible = VisibleMeshes.Where(p => p.Id == id).Any();
            if (isVisible)
            {
                if (!partAlreadyVisible)
                {
                    VisibleMeshes.Add(AllParts.Find(p => p.Id == id)!);
                }
            }
            else if (partAlreadyVisible)
            {
                VisibleMeshes.Remove(VisibleMeshes.Where(p => p.Id == id).First());
            }
        }

        public void Close()
        {
            foreach (GroundPartModel partModel in AllParts)
            {
                foreach (var m in partModel.LowMeshes)
                {
                    m.Mesh.ClearAllGeometryData();
                }
                foreach (var m in partModel.HighMeshes)
                {
                    m.Mesh.ClearAllGeometryData();
                }
            }
            Materials.Clear();
            ListItems.Clear();
            AllParts.Clear();
            VisibleMeshes.Clear();
        }
    }

    public class PartListItem : INotifyPropertyChanged
    {
        public int Id;
        public bool IsVisible
        {
            get;
            set { field = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
