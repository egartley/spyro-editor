using System;
using System.Collections.Generic;
using System.Text;

namespace Spyro_Editor.Utils
{
    public class GeometryDecode
    {
        public static int[] DecodeVertex(byte[] b, short x, short y, short z)
        {
            return [
                ((b[2] >> 5) | (b[3] << 3)) + x,
                ((b[1] >> 2) | ((b[2] & 31) << 6)) + y,
                (b[0] | ((b[1] & 3) << 8)) + z
            ];
        }

        public static int[] DecodeLowPoly(byte[] b)
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
    }
}
