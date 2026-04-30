using Spyro_Editor.Data.Level;

namespace Spyro_Editor.Utils
{
    public class TextureDecode
    {
        public static byte[] DecodeTexture(VRAM vram, TextureHeader header)
        {
            byte[] rgba = new byte[header.size * header.size * 4];
            if (header.m == 4)
            {
                int x4 = header.x4 >> 2;
                byte[][] clut = GetCLUT(vram, header.pageX, header.pageY, 16);
                for (int y = 0; y < header.size; y++)
                {
                    for (int x = 0; x < header.size / 4; x++)
                    {
                        ushort word = vram.GetWord(x4 + x, header.y4 + y);
                        for (byte nib = 0; nib < 4; nib++)
                        {
                            int i = (y * header.size + (x * 4 + nib)) * 4;
                            byte[] pixel = clut[(word >> (nib * 4)) & 15];
                            rgba[i] = pixel[0];
                            rgba[i + 1] = pixel[1];
                            rgba[i + 2] = pixel[2];
                            rgba[i + 3] = pixel[3];
                        }
                    }
                }
            }
            else if (header.m == 8)
            {
                int x4 = header.x4 >> 1;
                byte[][] clut = GetCLUT(vram, header.pageX, header.pageY, 256);
                for (int y = 0; y < header.size; y++)
                {
                    for (int x = 0; x < header.size / 2; x++)
                    {
                        ushort word = vram.GetWord(x4 + x, header.y4 + y);

                        int i1 = (y * header.size + (x * 2)) * 4;
                        byte[] pixel1 = clut[word & 256];
                        rgba[i1] = pixel1[0];
                        rgba[i1 + 1] = pixel1[1];
                        rgba[i1 + 2] = pixel1[2];
                        rgba[i1 + 3] = pixel1[3];

                        int i2 = (y * header.size + (x * 2 + 1)) * 4;
                        byte[] pixel2 = clut[(word >> 8) & 255];
                        rgba[i2] = pixel2[0];
                        rgba[i2 + 1] = pixel2[1];
                        rgba[i2 + 2] = pixel2[2];
                        rgba[i2 + 3] = pixel2[3];
                    }
                }
            }
            else
            {
                for (int y = 0; y < header.size; y++)
                {
                    for (int x = 0; x < header.size; x++)
                    {
                        ushort word = vram.GetWord(header.x4 + x, header.y4 + y);
                        int i = (y * header.size + x) * 4;
                        byte[] pixel = GetColorsByWord(word);
                        rgba[i] = pixel[0];
                        rgba[i + 1] = pixel[1];
                        rgba[i + 2] = pixel[2];
                        rgba[i + 3] = pixel[3];
                    }
                }
            }
            return rgba;
        }

        public static byte[] RotateTexture(byte[] rgba, TextureHeader header)
        {
            byte[] rotated = new byte[rgba.Length];

            switch (header.rotation)
            {
                case 0:
                    return rgba;
                case 1:
                    rotated = Rotate90(rgba, header.size);
                    break;
                case 2:
                    rotated = Rotate90(Rotate90(Rotate90(rgba, header.size), header.size), header.size);
                    break;
                case 3:
                    rotated = Rotate90(Rotate90(rgba, header.size), header.size);
                    break;
                case 4:
                    rotated = Rotate90(MirrorX(rgba, header.size), header.size);
                    break;
                case 5:
                    rotated = MirrorX(rgba, header.size);
                    break;
                case 6:
                    rotated = Rotate90(Rotate90(Rotate90(MirrorX(rgba, header.size), header.size), header.size), header.size);
                    break;
                case 7:
                    rotated = Rotate90(Rotate90(rgba, header.size), header.size);
                    break;
            }

            return rotated;
        }

        private static byte[][] GetCLUT(VRAM vram, int pageX, int pageY, int n)
        {
            byte[][] clut = new byte[n][];
            for (int i = 0; i < n; i++)
            {
                clut[i] = GetColorsByWord(vram.GetWord((pageY * 512 + pageX) + i));
            }
            return clut;
        }

        private static byte[] GetColorsByWord(int word)
        {
            return [
                (byte)(((word & 31) * 255 / 31) | 0),
                (byte)((((word >> 5) & 31) * 255 / 31) | 0),
                (byte)((((word >> 10) & 31) * 255 / 31) | 0),
                (byte)(((word >> 15) & 1) == 1 ? 0 : 255) // check this again
            ];
        }

        private static byte[] Rotate90(byte[] rgba, int size)
        {
            byte[] rotated = new byte[rgba.Length];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int s = (y * size + x) * 4;
                    int d = (x * size + (size - 1 - y)) * 4;
                    rotated[d] = rgba[s];
                    rotated[d + 1] = rgba[s + 1];
                    rotated[d + 2] = rgba[s + 2];
                    rotated[d + 3] = rgba[s + 3];
                }
            }
            return rotated;
        }

        private static byte[] MirrorX(byte[] rgba, int size)
        {
            byte[] rotated = new byte[rgba.Length];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int s = (y * size + x) * 4;
                    int d = (y * size + (size - 1 - x)) * 4;
                    rotated[d] = rgba[s];
                    rotated[d + 1] = rgba[s + 1];
                    rotated[d + 2] = rgba[s + 2];
                    rotated[d + 3] = rgba[s + 3];
                }
            }
            return rotated;
        }
    }
}
