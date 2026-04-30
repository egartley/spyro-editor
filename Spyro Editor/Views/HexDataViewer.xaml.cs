using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using Spyro_Editor.Contexts;
using Spyro_Editor.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Spyro_Editor.Views;

public sealed partial class HexDataViewer : Page
{
    private Subfile? Subfile;
    private WindowId? WindowId;

    public HexDataViewer()
    {
        InitializeComponent();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        HexViewer.AsciiEncoding = Encoding.UTF8;
    }

    public async Task Load()
    {
        HexViewer.Clear();
        byte[] buffer = await Subfile!.GetBuffer(false);
        HexViewer.LoadBytes(buffer);
        TotalSizeText.Text = $"Size: {ToPrettySize(Subfile.Size)}";
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is SubfileContext context)
        {
            Subfile = context.Subfile;
            WindowId = context.WindowId;
            await Load();
        }
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        HexViewer.Clear();
        base.OnNavigatedFrom(e);
    }

    private async void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        FileSavePicker savePicker = new FileSavePicker((WindowId)WindowId!)
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "subfile.bin"
        };
        savePicker.FileTypeChoices.Add("Binary", new List<string>() { ".bin" });

        var file = await savePicker.PickSaveFileAsync();
        if (file is not null && Subfile is not null)
        {
            byte[] buffer = await Subfile.GetBuffer(false);
            await File.WriteAllBytesAsync(file.Path, buffer);
        }
    }

    private string ToPrettySize(uint bytes)
    {
        string[] sizeUnits = { "bytes", "KB", "MB", "GB" };
        double size = bytes;
        byte index = 0;

        while (size >= 1024 && index < sizeUnits.Length - 1)
        {
            size /= 1024;
            index++;
        }

        return $"{Math.Round(size, 2)} {sizeUnits[index]}";
    }
}
