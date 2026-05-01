using Spyro_Editor.Interfaces;
using System.IO;

namespace Spyro_Editor.Data.Level
{
    /// <summary>
    /// A list of <see cref="TextureHeader"/> groups. The texture count is at 0x4, and each group contains that count times the number in parentheses below:<br />
    /// <br />
    /// Layout for Spyro 1:<br />
    /// LOD(1)-MID(1), followed by SPR(1)-COR(4)-TNY(16)<br />
    /// <br />
    /// Layout for Spyro 2 and 3:<br />
    /// LOD(1)-MID(1)-COR(4)<br />
    /// <br />
    /// LOD is low LOD<br />
    /// MID is middle LOD<br />
    /// COR are the four corners of a texture at high LOD<br />
    /// TNY is the same as COR, but divided into 16 parts rather than four
    /// </summary>
    public class TextureTable : IBinaryObject
    {
        public byte Count;
        public TextureHeader[] LODHeaders = [];
        public TextureHeader[] MIDHeaders = [];
        public TextureHeader[] SPRHeaders = [];
        public TextureHeader[][] CORHeaders = [];
        public TextureHeader[][] TNYHeaders = [];

        public void Read(BinaryReader reader)
        {
            // starts with size/jump at 0x0, don't care about that here
            reader.BaseStream.Seek(4, SeekOrigin.Current);

            // max 128 tiles (?) so the count is only ever a byte
            Count = reader.ReadByte();
            reader.BaseStream.Seek(3, SeekOrigin.Current);

            LODHeaders = new TextureHeader[Count];
            MIDHeaders = new TextureHeader[Count];
            for (int i = 0; i < Count; i++)
            {
                LODHeaders[i] = new TextureHeader(reader.ReadBytes(8));
                MIDHeaders[i] = new TextureHeader(reader.ReadBytes(8));
            }

            SPRHeaders = new TextureHeader[Count];
            CORHeaders = new TextureHeader[Count][];
            TNYHeaders = new TextureHeader[Count][];
            for (int i = 0; i < Count; i++)
            {
                SPRHeaders[i] = new TextureHeader(reader.ReadBytes(8));

                TextureHeader[] COR = new TextureHeader[4];
                for (int j = 0; j < COR.Length; j++)
                {
                    COR[j] = new TextureHeader(reader.ReadBytes(8));
                }
                CORHeaders[i] = COR;

                TextureHeader[] TNY = new TextureHeader[16];
                for (int j = 0; j < TNY.Length; j++)
                {
                    TNY[j] = new TextureHeader(reader.ReadBytes(8));
                }
                TNYHeaders[i] = TNY;
            }
        }
    }

    /// <summary>
    /// An 8-byte header that determines how to decode the VRAM for the texture, among other properties
    /// </summary>
    public class TextureHeader
    {
        public int PageX;
        public int PageY;
        public byte M;
        public byte Size;
        public int X1;
        public int X2;
        public int X3;
        public int X4;
        public int Y1;
        public int Y2;
        public int Y3;
        public int Y4;
        public int Rotation;
        public int S;
        public bool F;

        internal TextureHeader(byte[] rawBytes)
        {
            if ((rawBytes[7] & 14) > 0 || (rawBytes[6] & 8) == 0)
            {
                F = true;
            }

            if ((rawBytes[6] & 96) > 0 || rawBytes[1] != rawBytes[5])
            {
                F = true;
            }
            if ((rawBytes[7] & 128) > 0)
            {
                Size = 32;
            }
            else
            {
                Size = 16;
            }

            if ((rawBytes[0] + Size - 1) != rawBytes[4] || rawBytes[0] > (256 - Size) || rawBytes[1] > (256 - Size))
            {
                F = true;
            }
            if ((rawBytes[7] & 1) > 0)
            {
                M = 15;
            }
            else if ((rawBytes[6] & 128) > 0)
            {
                M = 8;
            }
            else
            {
                M = 4;
            }

            S = rawBytes[6] & 7;
            switch (M)
            {
                case 4:
                    S *= 256;
                    break;
                case 8:
                    S *= 128;
                    break;
                case 15:
                    S *= 64;
                    break;
            }

            X4 = rawBytes[0] + S;
            X3 = rawBytes[4] + S;
            X1 = X4;
            X2 = X4 + Size;
            Y4 = rawBytes[1];
            if ((rawBytes[6] & 16) > 0)
            {
                Y4 += 256;
            }
            Y3 = Y4;
            Y1 = Y4 + Size;
            Y2 = Y4 + Size;
            PageX = (rawBytes[2] & 31) * 16;
            PageY = (rawBytes[2] >> 6) | (rawBytes[3] << 2);
            Rotation = ((rawBytes[7] & 127) >> 4) & 7;
        }
    }
}
