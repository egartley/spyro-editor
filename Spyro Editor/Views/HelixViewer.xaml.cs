using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SharpDX;
using Spyro_Editor.Contexts;
using Spyro_Editor.Models;

namespace Spyro_Editor.Views
{
    public sealed partial class HelixViewer : Page
    {
        private HelixModel Model;

        public HelixViewer()
        {
            InitializeComponent();
            Model = new HelixModel();

            PerspectiveCamera camera = new PerspectiveCamera();
            camera.LookDirection = new Vector3(-1.5f, -1.5f, -2.5f);
            camera.Position = new Vector3(1.5f, 1.5f, 2.5f);
            Viewport.Camera = camera;

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
