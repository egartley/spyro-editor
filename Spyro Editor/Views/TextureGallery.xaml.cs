using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using Spyro_Editor.Contexts;
using Spyro_Editor.Data.Level;
using Spyro_Editor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Graphics.Imaging;

namespace Spyro_Editor.Views
{
    public sealed partial class TextureGallery : Page
    {
        private WindowId? WindowId;
        private Texture[]? Textures;
        public TextureGalleryModel Model;

        public TextureGallery()
        {
            InitializeComponent();
            Model = new TextureGalleryModel();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SubfileContext context)
            {
                WindowId = context.WindowId;
                Textures = context.Subfile.Level!.Textures!;
                Model.Load(Textures);
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Model.Items.Clear();
            Model.DetailGroupItems.Clear();
            Textures = [];
            base.OnNavigatedFrom(e);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is TextureGalleryItem item)
            {
                NoTextureSelectedText.Visibility = Visibility.Collapsed;
                DetailPanel.Visibility = Visibility.Visible;
                Model.LoadDetails(Textures![item.Index], item.Index);
            }
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button exportButton)
            {
                if (exportButton.DataContext is TextureDetailGroupItem item)
                {
                    FileSavePicker savePicker = new FileSavePicker((WindowId)WindowId!)
                    {
                        SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                        SuggestedFileName = $"texture_{item.Index}_{item.GroupName.ToLower()}"
                    };
                    savePicker.FileTypeChoices.Add("PNG", new List<string>() { ".png" });

                    var file = await savePicker.PickSaveFileAsync();
                    if (file is not null)
                    {
                        using (var stream = File.OpenWrite(file.Path))
                        {
                            using (var ramStream = stream.AsRandomAccessStream())
                            {
                                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ramStream);
                                encoder.SetSoftwareBitmap
                                (
                                    SoftwareBitmap.CreateCopyFromBuffer(item.Bitmap.PixelBuffer, BitmapPixelFormat.Bgra8, item.Bitmap.PixelWidth, item.Bitmap.PixelHeight)
                                );
                                await encoder.FlushAsync();
                            }
                        }
                    }
                }
            }
        }
    }
}
