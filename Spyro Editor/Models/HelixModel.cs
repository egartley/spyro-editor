using SharpDX;
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

        public async Task Load(Level level)
        {
            VisibleMeshes.Clear();
            AllParts.Clear();
            ListItems.Clear();
            for (int i = 0; i < level.Ground!.Parts.Length; i++)
            {
                GroundPartModel m = new GroundPartModel(level.Ground!.Parts[i], i);
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
            AllParts.ForEach(p => p.Mesh!.ClearAllGeometryData());
            AllParts.Clear();
            VisibleMeshes.Clear();
            ListItems.Clear();
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
