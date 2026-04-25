using Microsoft.UI.Xaml.Controls;
using System.Text;

namespace Spyro_Editor.Views;

public sealed partial class SubfileHexViewer : Page
{
    public SubfileHexViewer()
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

    public void Close()
    {
        HexViewer.Clear();
    }
}
