using Spyro_Editor.Constants;
using Spyro_Editor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spyro_Editor.Models
{
    public class WADModel
    {
        public List<WAD> WADs = [];
        public ObservableCollection<WADTreeNode> Nodes = new ObservableCollection<WADTreeNode>();

        public void AddWAD(WAD wad)
        {
            WADs.Add(wad);
            WADTreeNode node = new WADTreeNode() { DisplayName = wad.DisplayName, IsExpanded = true };
            WADTreeNode levels = new WADTreeNode() { DisplayName = "Levels", IsExpanded = true };
            WADTreeNode cutscenes = new WADTreeNode() { DisplayName = "Cutscenes"};
            WADTreeNode flyovers = new WADTreeNode() { DisplayName = "Credits Flyovers" };
            WADTreeNode overlays = new WADTreeNode() { DisplayName = "Overlays" };
            WADTreeNode other = new WADTreeNode() { DisplayName = "Other" };
            foreach (Subfile sf in wad.Subfiles)
            {
                WADTreeNode n = new WADTreeNode() { DisplayName = sf.DisplayName };
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
            node.Children.Add(levels);
            node.Children.Add(cutscenes);
            node.Children.Add(flyovers);
            node.Children.Add(overlays);
            node.Children.Add(other);
            Nodes.Add(node);
        }
    }

    public class WADTreeNode
    {
        public bool IsExpanded = false;
        public required string DisplayName;
        public ObservableCollection<WADTreeNode> Children = new ObservableCollection<WADTreeNode>();

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
