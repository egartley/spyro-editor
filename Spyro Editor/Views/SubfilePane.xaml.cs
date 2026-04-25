using Microsoft.UI.Xaml.Controls;
using Spyro_Editor.Data;
using System.IO;

namespace Spyro_Editor.Views
{
    public sealed partial class SubfilePane : Page
    {
        private string? WADPath;
        private Subfile? Subfile;
        private SubfileHexViewer? HexViewer;

        public SubfilePane()
        {
            InitializeComponent();
        }

        public async void Load(string wadPath, Subfile subfile)
        {
            WADPath = wadPath;
            Subfile = subfile;

            byte[] buffer = new byte[Subfile.Size];
            using (var stream = File.Open(WADPath, FileMode.Open))
            {
                stream.Seek(Subfile.Offset, SeekOrigin.Begin);
                await stream.ReadExactlyAsync(buffer, 0, (int)Subfile.Size);
            }

            if (HexViewer is null)
            {
                HexViewer = new SubfileHexViewer();
            }
            HexViewer.Load(buffer);
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
