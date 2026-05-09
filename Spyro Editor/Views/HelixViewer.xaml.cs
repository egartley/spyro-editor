using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;
using Microsoft.UI.Xaml;
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

            Viewport.Camera = new PerspectiveCamera() { FarPlaneDistance = 1e6, NearPlaneDistance = 10 };
            Viewport.EffectsManager = new DefaultEffectsManager();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SubfileContext context)
            {
                await Model.Load(context.Subfile.Level!);
                Viewport.Camera.Position = Model.Centroid;
                PartsPaneTitle.Text = $"Parts ({Model.VisibleMeshes.Count})";
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Model.Close();
            Viewport.EffectsManager.Dispose();
            base.OnNavigatedFrom(e);
        }

        private void PartListItemCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.DataContext is PartListItem part)
                {
                    Model.UpdateMeshVisibility(part.Id, part.IsVisible);
                }
            }
        }

        private void ShowAllPartsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(PartListItem item in Model.ListItems)
            {
                item.IsVisible = true;
                Model.UpdateMeshVisibility(item.Id, true);
            }
        }

        private void HideAllPartsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (PartListItem item in Model.ListItems)
            {
                item.IsVisible = false;
                Model.UpdateMeshVisibility(item.Id, false);
            }
        }
    }
}
