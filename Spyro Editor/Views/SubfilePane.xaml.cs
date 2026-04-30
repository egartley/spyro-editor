using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Spyro_Editor.Contexts;
using Spyro_Editor.Data;
using System;

namespace Spyro_Editor.Views
{
    public sealed partial class SubfilePane : Page
    {
        private int SelectedIndex;
        private WindowId? WindowId;
        private Subfile? Subfile;

        public SubfilePane()
        {
            InitializeComponent();
            SelectedIndex = 0;
        }

        public async void Load(string wadPath, Subfile subfile)
        {
            Subfile = subfile;

            await Subfile.WriteTemp(wadPath);

            Navigate(0);
        }

        public async void Close()
        {
            // dummy navigation to clear current page
            ContentFrame.Navigate(typeof(Page));
            await Subfile!.DeleteTemp();
        }

        private void Navigate(int index)
        {
            Type pageType;
            switch (index)
            {
                case 0:
                    pageType = typeof(HexDataViewer);
                    break;
                default:
                    pageType = typeof(HelixViewer);
                    break;
            }
            var effect = index - SelectedIndex > 0 ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft;
            ContentFrame.Navigate(pageType, new SubfileContext(Subfile!, (WindowId)WindowId!), new SlideNavigationTransitionInfo() { Effect = effect });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SubfilePaneContext context)
            {
                Load(context.WADPath, context.Subfile);
                WindowId = context.WindowId;
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Close();
            base.OnNavigatedFrom(e);
        }

        private void SelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs? args)
        {
            int newSelectedIndex = sender.Items.IndexOf(sender.SelectedItem);
            if (newSelectedIndex != SelectedIndex)
            {
                Navigate(newSelectedIndex);
                SelectedIndex = newSelectedIndex;
            }
        }
    }
}
