using Spyro_Editor.Utils;

namespace Spyro_Editor.Data.Level
{
    public class Texture
    {
        public static byte LOD_SIZE = 32;
        public static byte MID_SIZE = 32;
        public static byte SPR_SIZE = 32;
        public static byte COR_SIZE = 64;
        public static byte TNY_SIZE = 64;
        public byte[] RGBA_LOD;
        public byte[] RGBA_MID;
        public byte[] RGBA_SPR;
        public byte[] RGBA_COR;
        public byte[] RGBA_TNY;

        public Texture(VRAM vram, TextureHeader lodHeader, TextureHeader midHeader, TextureHeader sprHeader, TextureHeader[] corHeaders, TextureHeader[] tnyHeaders)
        {
            RGBA_LOD = TextureDecode.Rotate(TextureDecode.Decode(vram, lodHeader), lodHeader);
            RGBA_MID = TextureDecode.Rotate(TextureDecode.Decode(vram, midHeader), midHeader);
            RGBA_SPR = TextureDecode.Rotate(TextureDecode.Decode(vram, sprHeader), sprHeader);

            byte[][] cors = new byte[4][];
            for (int i = 0; i < cors.Length; i++)
            {
                cors[i] = TextureDecode.Rotate(TextureDecode.Decode(vram, corHeaders[i]), corHeaders[i]);
            }
            RGBA_COR = ArrayHelpers.CombineCorners(cors[0], cors[1], cors[2], cors[3], 32);

            byte[][] tnys = new byte[16][];
            for (int i = 0; i < tnys.Length; i++)
            {
                tnys[i] = TextureDecode.Rotate(TextureDecode.Decode(vram, tnyHeaders[i]), tnyHeaders[i]);
            }
            byte[] topLeft = ArrayHelpers.CombineCorners(tnys[0], tnys[1], tnys[4], tnys[5], 16);
            byte[] topRight = ArrayHelpers.CombineCorners(tnys[2], tnys[3], tnys[6], tnys[7], 16);
            byte[] bottomLeft = ArrayHelpers.CombineCorners(tnys[8], tnys[9], tnys[12], tnys[13], 16);
            byte[] bottomRight = ArrayHelpers.CombineCorners(tnys[10], tnys[11], tnys[14], tnys[15], 16);
            RGBA_TNY = ArrayHelpers.CombineCorners(topLeft, topRight, bottomLeft, bottomRight, 32);
        }
    }
}
