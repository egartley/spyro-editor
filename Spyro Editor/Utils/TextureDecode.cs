using Spyro_Editor.Data.Level;

namespace Spyro_Editor.Utils
{
    public class TextureDecode
    {
        public static byte[] Decode(VRAM vram, TextureHeader header)
        {
            byte[] rgba = new byte[header.Size * header.Size * 4];
            if (header.M == 4)
            {
                int x4 = header.X4 >> 2;
                byte[][] clut = GetCLUT(vram, header.PageX, header.PageY, 16);
                for (int y = 0; y < header.Size; y++)
                {
                    for (int x = 0; x < header.Size / 4; x++)
                    {
                        ushort word = vram.GetWord(x4 + x, header.Y4 + y);
                        for (byte nib = 0; nib < 4; nib++)
                        {
                            int i = (y * header.Size + (x * 4 + nib)) * 4;
                            byte[] pixel = clut[(word >> (nib * 4)) & 15];
                            rgba[i] = pixel[0];
                            rgba[i + 1] = pixel[1];
                            rgba[i + 2] = pixel[2];
                            rgba[i + 3] = pixel[3];
                        }
                    }
                }
            }
            else if (header.M == 8)
            {
                int x4 = header.X4 >> 1;
                byte[][] clut = GetCLUT(vram, header.PageX, header.PageY, 256);
                for (int y = 0; y < header.Size; y++)
                {
                    for (int x = 0; x < header.Size / 2; x++)
                    {
                        ushort word = vram.GetWord(x4 + x, header.Y4 + y);

                        int i1 = (y * header.Size + (x * 2)) * 4;
                        byte[] pixel1 = clut[word & 256];
                        rgba[i1] = pixel1[0];
                        rgba[i1 + 1] = pixel1[1];
                        rgba[i1 + 2] = pixel1[2];
                        rgba[i1 + 3] = pixel1[3];

                        int i2 = (y * header.Size + (x * 2 + 1)) * 4;
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
                for (int y = 0; y < header.Size; y++)
                {
                    for (int x = 0; x < header.Size; x++)
                    {
                        ushort word = vram.GetWord(header.X4 + x, header.Y4 + y);
                        int i = (y * header.Size + x) * 4;
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

        public static byte[] Rotate(byte[] rgba, TextureHeader header)
        {
            if (header.Rotation == 0)
            {
                return rgba;
            }

            byte[] rotated = new byte[rgba.Length];

            switch (header.Rotation)
            {
                case 1:
                    rotated = Mirror(Flip(Turn(rgba, header.Size), header.Size), header.Size);
                    break;
                case 2:
                    rotated = Turn(Turn(rgba, header.Size), header.Size);
                    break;
                case 3:
                    rotated = Mirror(Flip(rgba, header.Size), header.Size);
                    break;
                case 4:
                    rotated = Mirror(Turn(rgba, header.Size), header.Size);
                    break;
                case 5:
                    rotated = Mirror(rgba, header.Size);
                    break;
                case 6:
                    rotated = Flip(Turn(rgba, header.Size), header.Size);
                    break;
                case 7:
                    rotated = Flip(rgba, header.Size);
                    break;
                default:
                    // shouldn't get here!
                    return rgba;
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

        private static byte[] Turn(byte[] rgba, int size)
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

        private static byte[] Mirror(byte[] rgba, int size)
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

        private static byte[] Flip(byte[] rgba, int size)
        {
            byte[] rotated = new byte[rgba.Length];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int s = (y * size + x) * 4;
                    int d = ((size - 1 - y) * size + x) * 4;
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
