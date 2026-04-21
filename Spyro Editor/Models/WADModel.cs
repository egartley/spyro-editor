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
            WADTreeNode node = new WADTreeNode() { DisplayName = wad.DisplayName };
            foreach (Subfile sf in wad.Subfiles)
            {
                node.Children.Add(new WADTreeNode() { DisplayName = sf.DisplayName });
            }
            Nodes.Add(node);
        }
    }

    public class WADTreeNode
    {
        public string DisplayName;
        public ObservableCollection<WADTreeNode> Children = new ObservableCollection<WADTreeNode>();

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
