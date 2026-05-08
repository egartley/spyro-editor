using Spyro_Editor.Constants;
using System.IO;

namespace Spyro_Editor.Interfaces
{
    public interface IBinaryObject
    {
        void Read(BinaryReader reader, Game game);
    }
}
