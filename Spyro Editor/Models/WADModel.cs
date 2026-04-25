using Spyro_Editor.Constants;
using Spyro_Editor.Data;
using Spyro_Editor.Views;
using System.Collections.ObjectModel;

namespace Spyro_Editor.Models
{
    public class WADModel
    {
        public ObservableCollection<WADTreeNode> Nodes = new ObservableCollection<WADTreeNode>();
        private WAD? WAD;

        public void Load(WAD wad)
        {
            WAD = wad;
            Nodes.Clear();
            WADTreeNode levels = new WADTreeNode() { DisplayName = "Levels", Glyph = "\uF158", Type = WADTreeNode.NodeType.Group, IsExpanded = true };
            WADTreeNode cutscenes = new WADTreeNode() { DisplayName = "Cutscenes", Glyph = "\uE8B2", Type = WADTreeNode.NodeType.Group };
            WADTreeNode flyovers = new WADTreeNode() { DisplayName = "Credits Flyovers", Glyph = "\uEA37", Type = WADTreeNode.NodeType.Group };
            WADTreeNode overlays = new WADTreeNode() { DisplayName = "Overlays", Glyph = "\uEEA1", Type = WADTreeNode.NodeType.Group };
            WADTreeNode other = new WADTreeNode() { DisplayName = "Other", Glyph = "\uE8A5", Type = WADTreeNode.NodeType.Group };
            foreach (Subfile sf in wad.Subfiles)
            {
                WADTreeNode n = new WADTreeNode() { DisplayName = sf.DisplayName, Id = sf.Id, Type = WADTreeNode.NodeType.Subfile };
                switch (sf.Type)
                {
                    case SubfileType.Level:
                        levels.Children.Add(n);
                        break;
                    case SubfileType.Cutscene:
                        cutscenes.Children.Add(n);
                        break;
                    case SubfileType.Flyover:
                        flyovers.Children.Add(n);
                        break;
                    case SubfileType.Overlay:
                        overlays.Children.Add(n);
                        break;
                    default:
                        other.Children.Add(n);
                        break;
                }
            }
            Nodes.Add(levels);
            Nodes.Add(cutscenes);
            Nodes.Add(flyovers);
            Nodes.Add(overlays);
            Nodes.Add(other);
        }

        public void Unload()
        {
            Nodes.Clear();
            WAD = null;
        }

        public WAD GetWAD()
        {
            return WAD!;
        }

        public Subfile? GetSubfile(byte id)
        {
            return WAD!.Subfiles.Find(s => s.Id == id);
        }
    }
}
