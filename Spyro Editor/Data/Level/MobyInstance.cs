using Spyro_Editor.Constants;
using Spyro_Editor.Interfaces;
using System.IO;

namespace Spyro_Editor.Data.Level
{
    public class MobyInstance : IBinaryObject
    {
        public int X;
        public int Y;
        public int Z;
        public byte ClassID;
        public byte Yaw;
        public static byte SIZE = 88;

        public void Read(BinaryReader reader, Game game)
        {
            reader.BaseStream.Seek(12, SeekOrigin.Current);
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            Z = reader.ReadInt32();
            reader.BaseStream.Seek(30, SeekOrigin.Current);
            ClassID = reader.ReadByte();
            reader.BaseStream.Seek(15, SeekOrigin.Current);
            Yaw = reader.ReadByte();
        }
    }
}
