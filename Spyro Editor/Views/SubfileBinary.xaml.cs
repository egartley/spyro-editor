using Microsoft.UI.Xaml.Controls;
using System.Text;

namespace Spyro_Editor.Views
{
    public sealed partial class SubfileBinary : Page
    {
        public SubfileBinary()
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            HexViewer.AsciiEncoding = Encoding.UTF8;
        }

        public void Load(byte[] data)
        {
            HexViewer.Clear();
            HexViewer.LoadBytes(data);
        }
    }
}
