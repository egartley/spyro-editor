using Spyro_Editor.Interfaces;
using System;
using System.IO;

namespace Spyro_Editor.Data.Level
{
    public class TextureTable : IBinaryObject
    {
        public TextureHeader[] Headers;

        public TextureTable()
        {
            Headers = [];
        }

        public void Read(BinaryReader reader)
        {
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            uint tileCount = reader.ReadUInt32();
            Headers = new TextureHeader[tileCount];
            for (var i = 0; i < tileCount; i++)
            {
                reader.BaseStream.Seek(8, SeekOrigin.Current);
                Headers[i] = new TextureHeader(reader.ReadBytes(8));
            }
        }
    }

    public class TextureHeader
    {
        public int pageX;
        public int pageY;
        public byte m;
        public byte size;
        public int x1;
        public int x2;
        public int x3;
        public int x4;
        public int y1;
        public int y2;
        public int y3;
        public int y4;
        public int rotation;
        public int s;
        public bool f;

        internal TextureHeader(byte[] rawBytes)
        {
            if ((rawBytes[7] & 14) > 0 || (rawBytes[6] & 8) == 0)
            {
                f = true;
            }

            if ((rawBytes[6] & 96) > 0 || rawBytes[1] != rawBytes[5])
            {
                f = true;
            }
            if ((rawBytes[7] & 128) > 0)
            {
                size = 32;
            }
            else
            {
                size = 16;
            }

            if ((rawBytes[0] + size - 1) != rawBytes[4] || rawBytes[0] > (256 - size) || rawBytes[1] > (256 - size))
            {
                f = true;
            }
            if ((rawBytes[7] & 1) > 0)
            {
                m = 15;
            }
            else if ((rawBytes[6] & 128) > 0)
            {
                m = 8;
            }
            else
            {
                m = 4;
            }

            s = rawBytes[6] & 7;
            switch (m)
            {
                case 4:
                    s *= 256;
                    break;
                case 8:
                    s *= 128;
                    break;
                case 15:
                    s *= 64;
                    break;
            }

            x4 = rawBytes[0] + s;
            x3 = rawBytes[4] + s;
            x1 = x4;
            x2 = x4 + size;
            y4 = rawBytes[1];
            if ((rawBytes[6] & 16) > 0)
            {
                y4 += 256;
            }
            y3 = y4;
            y1 = y4 + size;
            y2 = y4 + size;
            pageX = (rawBytes[2] & 31) * 16;
            pageY = (rawBytes[2] >> 6) | (rawBytes[3] << 2);
            rotation = ((rawBytes[7] & 127) >> 4) & 7;
        }
    }
}
