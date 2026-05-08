using Microsoft.UI.Xaml.Media.Imaging;
using Spyro_Editor.Data.Level;
using Spyro_Editor.Utils;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Spyro_Editor.Models
{
    public class TextureGalleryModel
    {
        public ObservableCollection<TextureGalleryItem> Items = new ObservableCollection<TextureGalleryItem>();
        public ObservableCollection<TextureDetailGroupItem> DetailGroupItems = new ObservableCollection<TextureDetailGroupItem>();

        public void Load(Texture[] textures)
        {
            Items.Clear();
            for(int i = 0; i < textures.Length; i++)
            {
                Items.Add(new TextureGalleryItem(textures[i], i));
            }
        }

        public void LoadDetails(Texture texture, int index)
        {
            DetailGroupItems.Clear();
            DetailGroupItems.Add(new TextureDetailGroupItem(index, texture.RGBA_COR, "COR", Texture.COR_SIZE, Texture.COR_SIZE, "Made up of 4 separate 32x32 textures"));
            if (texture.RGBA_TNY.Length > 0)
            {
                DetailGroupItems.Add(new TextureDetailGroupItem(index, texture.RGBA_TNY, "TNY", Texture.TNY_SIZE, Texture.TNY_SIZE, "Made up of 16 separate 16x16 textures"));
            }
            DetailGroupItems.Add(new TextureDetailGroupItem(index, texture.RGBA_MID, "MID", Texture.MID_SIZE, Texture.MID_SIZE, "Shown at mid-range to the camera"));
            DetailGroupItems.Add(new TextureDetailGroupItem(index, texture.RGBA_LOD, "LOD", Texture.LOD_SIZE, Texture.LOD_SIZE, "Shown at farther distances from the camera"));
            if (texture.RGBA_SPR.Length > 0)
            {
                DetailGroupItems.Add(new TextureDetailGroupItem(index, texture.RGBA_SPR, "SPR", Texture.SPR_SIZE, Texture.SPR_SIZE, "Unknown usage, same appearance as MID"));
            }
        }
    }

    public class TextureGalleryItem
    {
        public int Index;
        public string DisplayText;
        public WriteableBitmap Bitmap;

        public TextureGalleryItem(Texture texture, int index)
        {
            Bitmap = new WriteableBitmap(64, 64);
            using (var stream = Bitmap.PixelBuffer.AsStream())
            {
                byte[] bgra = ArrayHelpers.ConvertRGBAToBGRA(texture.RGBA_COR);
                stream.Write(bgra, 0, bgra.Length);
            }
            Index = index;
            DisplayText = $"{index}";
        }
    }

    public class TextureDetailGroupItem
    {
        public int Index;
        public byte ImageWidth;
        public byte ImageHeight;
        public string GroupName;
        public string Dimensions;
        public string GroupDescription;
        public WriteableBitmap Bitmap;

        public TextureDetailGroupItem(int index, byte[] rgba, string group, byte width, byte height, string description)
        {
            Index = index;
            ImageWidth = width;
            ImageHeight = height;
            Bitmap = new WriteableBitmap(width, height);
            using (var stream = Bitmap.PixelBuffer.AsStream())
            {
                byte[] bgra = ArrayHelpers.ConvertRGBAToBGRA(rgba);
                stream.Write(bgra, 0, bgra.Length);
            }
            Dimensions = $"{width} x {height}";
            GroupName = group;
            GroupDescription = description;
        }
    }
}
