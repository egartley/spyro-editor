using Microsoft.UI.Xaml.Controls;
using Spyro_Editor.Models;

namespace Spyro_Editor.Views
{
    public sealed partial class WADView : Page
    {
        public WADModel Model = new WADModel();

        public WADView()
        {
            InitializeComponent();
        }
    }
}
