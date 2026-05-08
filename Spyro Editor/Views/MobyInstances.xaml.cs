using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spyro_Editor.Contexts;
using Spyro_Editor.Models;

namespace Spyro_Editor.Views
{
    public sealed partial class MobyInstances : Page
    {
        public MobyInstancesModel Model;

        public MobyInstances()
        {
            InitializeComponent();
            Model = new();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SubfileContext context)
            {
                Model.Load(context.Subfile.Level!.MobyInstances!);
                HeaderTextBlock.Text = $"All Instances ({Model.Items.Count})";
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Model.Items.Clear();
            base.OnNavigatedFrom(e);
        }
    }
}
