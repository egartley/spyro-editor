using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using Spyro_Editor.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spyro_Editor.Views;

public sealed partial class SubfileHexViewer : Page
{
    private string WADPath;
    private WindowId WindowId;
    private Subfile? Subfile;

    public SubfileHexViewer(WindowId windowId)
    {
        InitializeComponent();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        HexViewer.AsciiEncoding = Encoding.UTF8;
        WADPath = "";
        WindowId = windowId;
    }

    public async void Load(string wadPath, Subfile subfile)
    {
        WADPath = wadPath;
        Subfile = subfile;
        byte[] buffer = await Subfile.GetBuffer(WADPath);
        HexViewer.Clear();
        HexViewer.LoadBytes(buffer);
        TotalSizeText.Text = $"Size: {ToPrettySize(Subfile.Size)}";
    }

    public void Close()
    {
        HexViewer.Clear();
    }

    private async void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        FileSavePicker savePicker = new FileSavePicker(WindowId)
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "subfile.bin"
        };
        savePicker.FileTypeChoices.Add("Binary", new List<string>() { ".bin" });

        var file = await savePicker.PickSaveFileAsync();
        if (file is not null && Subfile is not null)
        {
            byte[] buffer = await Subfile.GetBuffer(WADPath);
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
