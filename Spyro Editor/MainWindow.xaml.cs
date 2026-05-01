using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using Spyro_Editor.Contexts;
using Spyro_Editor.Data;
using Spyro_Editor.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Spyro_Editor
{
    public sealed partial class MainWindow : Window
    {
        private WADBrowser WADBrowser;

        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(MainTitleBar);

            WADBrowser = new WADBrowser(this);

            MainSplitView.Pane = WADBrowser;
            SplitViewFrame.Navigate(typeof(GetStartedPane), new GetStartedContext(this, false));
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
                using (var stream = File.Open(result.Path, FileMode.Open))
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

        private void ExitFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
