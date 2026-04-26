using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Spyro_Editor.Data;

namespace Spyro_Editor.Views
{
    public sealed partial class SubfilePane : Page
    {
        private WindowId WindowId;
        private Subfile? Subfile;
        private SubfileHexViewer? HexViewer;

        public SubfilePane(WindowId windowId)
        {
            InitializeComponent();
            WindowId = windowId;
        }

        public async void Load(string wadPath, Subfile subfile)
        {
            if (Subfile is not null)
            {
                Subfile.DeleteTemp();
            }
            Subfile = subfile;

            byte[] buffer = await Subfile.WriteTemp(wadPath);

            if (HexViewer is null)
            {
                HexViewer = new SubfileHexViewer(WindowId);
            }
            HexViewer.Load(buffer, subfile);

            ContentFrame.Content = HexViewer;
        }

        public void Close()
        {
            if (HexViewer is not null)
            {
                HexViewer.Close();
            }
            if (Subfile is not null)
            {
                Subfile.DeleteTemp();
            }
            Subfile = null;
            ContentFrame.Content = null;
        }

        private void SelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {

        }
    }
}
