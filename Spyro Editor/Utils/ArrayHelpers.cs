using System;

namespace Spyro_Editor.Utils
{
    public class ArrayHelpers
    {
        public static byte[] CombineCorners(byte[] topLeft, byte[] topRight, byte[] bottomLeft, byte[] bottomRight, int size)
        {
            int rowWidth = size * 4;
            byte[] combined = new byte[rowWidth * size * 4];
            int dstOffset = 0;
            for (int i = 0; i < size; i++)
            {
                int srcOffset = i * rowWidth;
                Buffer.BlockCopy(topLeft, srcOffset, combined, dstOffset, rowWidth);
                dstOffset += rowWidth;
                Buffer.BlockCopy(topRight, srcOffset, combined, dstOffset, rowWidth);
                dstOffset += rowWidth;
            }
            for (int i = 0; i < size; i++)
            {
                int srcOffset = i * rowWidth;
                Buffer.BlockCopy(bottomLeft, srcOffset, combined, dstOffset, rowWidth);
                dstOffset += rowWidth;
                Buffer.BlockCopy(bottomRight, srcOffset, combined, dstOffset, rowWidth);
                dstOffset += rowWidth;
            }
            return combined;
        }

        public static byte[] ConvertRGBAToBGRA(byte[] rgba)
        {
            byte[] output = new byte[rgba.Length];
            for (int i = 0; i < rgba.Length; i += 4)
            {
                byte r = rgba[i];
                byte g = rgba[i + 1];
                byte b = rgba[i + 2];
                byte a = rgba[i + 3];
                output[i] = b;
                output[i + 1] = g;
                output[i + 2] = r;
                output[i + 3] = a;
            }
            return output;
        }
    }
}
