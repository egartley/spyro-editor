using Spyro_Editor.Constants;
using Spyro_Editor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Devices.Radios;

namespace Spyro_Editor.Models
{
    public class WADModel
    {
        public ObservableCollection<WADTreeNode> Nodes = new ObservableCollection<WADTreeNode>();
        private List<WAD> WADs = [];

        public void Add(WAD wad)
        {
            WADs.Add(wad);
            WADTreeNode node = new WADTreeNode() { DisplayName = wad.DisplayName, Type = WADTreeNode.NodeType.Root, IsExpanded = true };
            WADTreeNode levels = new WADTreeNode() { DisplayName = "Levels", Type = WADTreeNode.NodeType.Group, IsExpanded = true };
            WADTreeNode cutscenes = new WADTreeNode() { DisplayName = "Cutscenes", Type = WADTreeNode.NodeType.Group };
            WADTreeNode flyovers = new WADTreeNode() { DisplayName = "Credits Flyovers", Type = WADTreeNode.NodeType.Group };
            WADTreeNode overlays = new WADTreeNode() { DisplayName = "Overlays", Type = WADTreeNode.NodeType.Group };
            WADTreeNode other = new WADTreeNode() { DisplayName = "Other", Type = WADTreeNode.NodeType.Group };
            foreach (Subfile sf in wad.Subfiles)
            {
                WADTreeNode n = new WADTreeNode() { DisplayName = sf.DisplayName, Id = sf.Id, WADId = sf.WADId, Type = WADTreeNode.NodeType.Subfile };
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

        public WAD? GetWAD(byte id)
        {
            return WADs.Find(w => w.Id == id);
        }

        public Subfile? GetSubfile(byte wadId, byte id)
        {
            var wad = GetWAD(wadId);
            if (wad is not null)
            {
                return wad.Subfiles.Find(s => s.Id == id);
            }
            return null;
        }
    }

    public class WADTreeNode
    {
        public enum NodeType
        {
            Root,
            Group,
            Subfile
        }

        public byte Id = 0;
        public byte WADId = 0;
        public bool IsExpanded = false;
        public required string DisplayName;
        public required NodeType Type;
        public ObservableCollection<WADTreeNode> Children = new ObservableCollection<WADTreeNode>();
    }
}
