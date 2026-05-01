using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spyro_Editor.Contexts;
using Spyro_Editor.Models;

namespace Spyro_Editor.Views
{
    public sealed partial class SubfileOverview : Page
    {
        private OverviewModel Model;

        public SubfileOverview()
        {
            InitializeComponent();
            Model = new OverviewModel();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SubfileContext context)
            {
                await Model.Load(context.Subfile);
            }
        }
    }
}
