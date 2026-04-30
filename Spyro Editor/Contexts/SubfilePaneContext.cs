using Microsoft.UI;
using Spyro_Editor.Data;

namespace Spyro_Editor.Contexts
{
    public class SubfilePaneContext
    {
        public string WADPath;
        public Subfile Subfile;
        public WindowId WindowId;

        public SubfilePaneContext(string wadPath, Subfile subfile, WindowId windowId)
        {
            WADPath = wadPath;
            Subfile = subfile;
            WindowId = windowId;
        }
    }
}
