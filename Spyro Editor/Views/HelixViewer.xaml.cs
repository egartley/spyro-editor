using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spyro_Editor.Contexts;
using Spyro_Editor.Models;

namespace Spyro_Editor.Views
{
    public sealed partial class HelixViewer : Page
    {
        private readonly HelixModel Model;

        public HelixViewer()
        {
            InitializeComponent();
            Model = new HelixModel();

            Viewport.Camera = new PerspectiveCamera();
            Viewport.EffectsManager = new DefaultEffectsManager();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SubfileContext context)
            {
                await Model.Load(context.Subfile);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Viewport.EffectsManager.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
