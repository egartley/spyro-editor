using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using Spyro_Editor.Contexts;
using Spyro_Editor.Data;
using Spyro_Editor.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Spyro_Editor
{
    public sealed partial class MainWindow : Window
    {
        private WADBrowser WADBrowser;
        private bool IsClosing;
        private bool IsCleanedUp;

        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(MainTitleBar);

            WADBrowser = new WADBrowser(this);

            MainSplitView.Pane = WADBrowser;
            SplitViewFrame.Navigate(typeof(GetStartedPane), new GetStartedContext(this, false));
            AppWindow.Closing += AppWindow_Closing;
        }

        private async void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            if (!IsCleanedUp)
            {
                args.Cancel = true;
                await CloseAfterCleanUp();
            }
        }

        private async Task CloseAfterCleanUp()
        {
            if (IsClosing)
            {
                return;
            }
            IsClosing = true;
            // dummy navigation so the current page cleans up
            SplitViewFrame.Navigate(typeof(Page));
            await SubfilePane.PendingClose;
            try
            {
                await Subfile.DeleteTempFolder();
            }
            catch (Exception)
            {
                // fine if temp folder is still there
            }
            IsCleanedUp = true;
            Close();
        }

        public async void OpenWAD()
        {
            FileOpenPicker opener = new FileOpenPicker(AppWindow.Id)
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeFilter = { ".wad" }
            };
            var result = await opener.PickSingleFileAsync();
            if (result is not null)
            {
                WAD wad;
                string path = result.Path;
                using (var stream = File.Open(result.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        wad = new WAD(reader, path);
                    }
                }
                WADBrowser.Load(wad);
                SplitViewFrame.Navigate(typeof(GetStartedPane), new GetStartedContext(this, true));
                CloseWADFlyoutItem.IsEnabled = true;
            }
        }

        private void CloseWAD()
        {
            SplitViewFrame.Navigate(typeof(GetStartedPane), new GetStartedContext(this, false));
            WADBrowser.Unload();
            CloseWADFlyoutItem.IsEnabled = false;
        }

        public void LoadSubfile(string wadPath, Subfile subfile)
        {
            SplitViewFrame.Navigate(typeof(SubfilePane), new SubfilePaneContext(wadPath, subfile, AppWindow.Id));
        }

        private void OpenWADFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            OpenWAD();
        }

        private void CloseWADFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            CloseWAD();
        }

        private void OpenGitHubFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/egartley/spyro-editor",
                UseShellExecute = true
            });
        }

        private async void AboutFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = rootGrid.XamlRoot,
                Title = "Spyro Editor",
                Content = $"Version {App.Version}",
                CloseButtonText = "Ok"
            };
            await dialog.ShowAsync();
        }

        private async void ExitFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await CloseAfterCleanUp();
        }
    }
}
