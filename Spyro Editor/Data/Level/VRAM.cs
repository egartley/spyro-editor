using Spyro_Editor.Constants;
using Spyro_Editor.Interfaces;
using System;
using System.IO;

namespace Spyro_Editor.Data.Level
{
    public class VRAM : IBinaryObject
    {
        private ushort[] Words;
        private static int SIZE = 524288;
        private static int MAX_WORD = 512;

        public VRAM()
        {
            Words = [];
        }

        public ushort GetWord(int i)
        {
            if (i < 0 || i >= Words.Length)
            {
                return 0;
            }
            return Words[i];
        }

        public ushort GetWord(int x, int y)
        {
            if (x < 0 || x >= MAX_WORD || y < 0 || y >= MAX_WORD)
            {
                return 0;
            }
            return Words[y * MAX_WORD + x];
        }

        public void ApplyFontStripFix()
        {
            for (int x = MAX_WORD; x <= 575; x++)
            {
                Words[255 * MAX_WORD + x] = Words[254 * MAX_WORD + (x - MAX_WORD)];
            }
        }

        public void Read(BinaryReader reader, Game game)
        {
            byte[] bytes = reader.ReadBytes(SIZE);
            Words = new ushort[bytes.Length / 2];
            for (int i = 0; i < Words.Length; i++)
            {
                Words[i] = BitConverter.ToUInt16(bytes, i * 2);
            }
        }
    }
}
