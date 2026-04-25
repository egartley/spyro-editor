using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spyro_Editor.Data;
using Spyro_Editor.Models;
using System.Collections.ObjectModel;

namespace Spyro_Editor.Views
{
    public sealed partial class WADBrowser : Page
    {
        private string NoWADText;
        private MainWindow Main;
        private WADModel Model;

        public WADBrowser(MainWindow main)
        {
            InitializeComponent();
            Main = main;
            Model = new WADModel();
            NoWADText = "(No WAD selected)";
            Title.Text = NoWADText;
        }

        public void Load(WAD wad)
        {
            Title.Text = wad.DisplayName; // "TITLE T"
            Model.Load(wad);
        }

        public void Unload()
        {
            Title.Text = NoWADText;
            Model.Unload();
        }

        private void WADTree_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            var node = args.InvokedItem as WADTreeNode;
            if (node is not null && node.Type == WADTreeNode.NodeType.Subfile)
            {
                Subfile sf = Model.GetSubfile(node.Id)!;
                Main.LoadSubfile(Model.GetWAD().Path, sf);
            }
        }
    }

    public class WADTreeNode
    {
        public enum NodeType
        {
            Group,
            Subfile
        }

        public byte Id = 0;
        public bool IsExpanded = false;
        public string Glyph = "";
        public required string DisplayName;
        public required NodeType Type;
        public ObservableCollection<WADTreeNode> Children = new ObservableCollection<WADTreeNode>();
    }

    class WADTreeNodeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? GroupTemplate { get; set; }

        public DataTemplate? SubfileTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item)
        {
            var node = (WADTreeNode) item;

            switch (node.Type)
            {
                case WADTreeNode.NodeType.Group:
                    return GroupTemplate;
                default:
                case WADTreeNode.NodeType.Subfile:
                    return SubfileTemplate;
            }
        }
    }
}
