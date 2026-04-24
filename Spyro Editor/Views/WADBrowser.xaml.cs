using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spyro_Editor.Data;
using Spyro_Editor.Models;

namespace Spyro_Editor.Views
{
    public sealed partial class WADBrowser : Page
    {
        private MainWindow Main;
        private WADModel Model;

        public WADBrowser(MainWindow main)
        {
            InitializeComponent();
            Main = main;
            Model = new WADModel();
        }

        public void Add(WAD wad)
        {
            Model.Add(wad);
        }

        private void WADTree_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            var node = args.InvokedItem as WADTreeNode;
            if (node is not null)
            {
                if (node.Type == WADTreeNode.NodeType.Subfile)
                {
                    var sf = Model.GetSubfile(node.WADId, node.Id);
                    if (sf is not null)
                    {
                        var wad = Model.GetWAD(node.WADId);
                        if (wad is not null)
                        {
                            Main.LoadSubfileBinary(wad.Path, sf);
                        }
                    }
                }
            }
        }
    }

    class WADTreeNodeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? RootTemplate { get; set; }

        public DataTemplate? GroupTemplate { get; set; }

        public DataTemplate? SubfileTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item)
        {
            var node = (WADTreeNode) item;

            switch (node.Type)
            {
                case WADTreeNode.NodeType.Root:
                    return RootTemplate;
                case WADTreeNode.NodeType.Group:
                    return GroupTemplate;
                case WADTreeNode.NodeType.Subfile:
                default:
                    return SubfileTemplate;
            }
        }
    }
}
