using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Spyro_Editor.Contexts;
using Spyro_Editor.Data;
using System;
using System.Threading.Tasks;

namespace Spyro_Editor.Views
{
    public sealed partial class SubfilePane : Page
    {
        public static Task PendingClose = Task.CompletedTask;
        private Task LoadTask = Task.CompletedTask;
        private bool IsClosed;
        private int SelectedIndex;
        private WindowId? WindowId;
        private Subfile? Subfile;

        public SubfilePane()
        {
            InitializeComponent();
            SelectedIndex = 0;
        }

        private async Task Load(string wadPath, Subfile subfile)
        {
            Subfile = subfile;
            try
            {
                // previous pane may still be using the same temp file
                await PendingClose;
                if (IsClosed)
                {
                    return;
                }
                await Subfile.WriteTemp(wadPath);
                if (Subfile.Type != Constants.SubfileType.Level)
                {
                    HelixViewSelectorItem.Visibility = Visibility.Collapsed;
                    TextureGallerySelectorItem.Visibility = Visibility.Collapsed;
                    MobyInstancesSelectorItem.Visibility = Visibility.Collapsed;
                }
                else
                {
                    await Subfile.LoadLevel();
                }
            }
            catch (Exception)
            {
                return;
            }
            if (IsClosed)
            {
                return;
            }

            SelectorBar.IsEnabled = true;
            Navigate(0, true);
        }

        private async Task Close()
        {
            IsClosed = true;
            // dummy navigation to clear current page
            ContentFrame.Navigate(typeof(Page));
            await LoadTask;
            if (Subfile is not null)
            {
                await Subfile.DeleteTemp();
            }
        }

        private void Navigate(int index, bool bottomTransition = false)
        {
            Type pageType;
            switch (index)
            {
                case 0:
                    pageType = typeof(SubfileOverview);
                    break;
                case 1:
                    pageType = typeof(HelixViewer);
                    break;
                case 2:
                    pageType = typeof(TextureGallery);
                    break;
                case 3:
                    pageType = typeof(MobyInstances);
                    break;
                default:
                    pageType = typeof(HexDataViewer);
                    break;
            }
            var effect = index - SelectedIndex > 0 ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft;
            ContentFrame.Navigate(pageType, new SubfileContext(Subfile!, (WindowId)WindowId!), new SlideNavigationTransitionInfo() { Effect = bottomTransition ? SlideNavigationTransitionEffect.FromBottom : effect });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SubfilePaneContext context)
            {
                WindowId = context.WindowId;
                LoadTask = Load(context.WADPath, context.Subfile);
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            PendingClose = Close();
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
