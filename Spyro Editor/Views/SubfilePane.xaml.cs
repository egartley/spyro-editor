using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Spyro_Editor.Data;

namespace Spyro_Editor.Views
{
    public sealed partial class SubfilePane : Page
    {
        private string WADPath;
        private WindowId WindowId;
        private Subfile? Subfile;
        private SubfileHexViewer? HexViewer;

        public SubfilePane(WindowId windowId)
        {
            InitializeComponent();
            WADPath = "";
            WindowId = windowId;
        }

        public async void Load(string wadPath, Subfile subfile)
        {
            WADPath = wadPath;
            Subfile = subfile;

            if (HexViewer is null)
            {
                HexViewer = new SubfileHexViewer(WindowId);
            }
            HexViewer.Load(wadPath, subfile);
            ContentFrame.Content = HexViewer;
        }

        public void Close()
        {
            if (HexViewer is not null)
            {
                HexViewer.Close();
            }
            Subfile = null;
            WADPath = "";
            ContentFrame.Content = null;
        }

        private void SelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {

        }
    }
}
