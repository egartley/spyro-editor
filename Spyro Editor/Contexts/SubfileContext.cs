using Microsoft.UI;
using Spyro_Editor.Data;

namespace Spyro_Editor.Contexts
{
    public class SubfileContext
    {
        public Subfile Subfile;
        public WindowId WindowId;

        public SubfileContext(Subfile subfile, WindowId windowId)
        {
            Subfile = subfile;
            WindowId = windowId;
        }

    }
}
