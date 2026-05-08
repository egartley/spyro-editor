using Spyro_Editor.Constants;

namespace Spyro_Editor.Utils
{
    public class GeometryDecode
    {
        public static int[] DecodeVertex(byte[] b, Game game, short baseX, short baseY, short baseZ)
        {
            int x = ((b[2] >> 5) | (b[3] << 3)) + baseX;
            int y = ((b[1] >> 2) | ((b[2] & 31) << 6)) + baseY;
            int z = b[0] | ((b[1] & 3) << 8);
            if (game != Game.Spyro1)
            {
                z = (z << 1) + baseZ;
            }
            else
            {
                z += baseZ;
            }
            // correction for z-up
            return [x, z, -1 * y];
        }

        public static int[] DecodeLowPoly(byte[] b, Game game)
        {
            if (game == Game.Spyro1)
            {
                return [
                    b[1] & 63,
                    (b[1] >> 6) | ((b[2] & 15) << 2),
                    (b[2] >> 4) | ((b[3] & 3) << 4),
                    b[3] >> 2,
                    b[5] & 63,
                    (b[5] >> 6) | ((b[6] & 15) << 2),
                    (b[6] >> 4) | ((b[7] & 3) << 4),
                    b[7] >> 2
                ];
            }
            else
            {
                return [
                    (b[0] >> 3) | ((b[1] & 3) << 5),
                    (b[1] >> 2) | ((b[2] & 1) << 6),
                    b[2] >> 1,
                    b[3] & 127,
                    (b[4] >> 4) | ((b[5] & 7) << 4),
                    (b[5] >> 3) | ((b[6] & 3) << 5),
                    (b[6] >> 2) | ((b[7] & 1) << 6),
                    b[7] >> 1
                ];
            }

        }
    }
}
