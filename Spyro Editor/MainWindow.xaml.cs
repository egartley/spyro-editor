using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
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
        private WADView wadView;
        private Version Version;

        public MainWindow()
        {
            InitializeComponent();
            Version = Assembly.GetEntryAssembly()!.GetName().Version!;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(mainTitleBar);

            wadView = new WADView();
            mainSplitView.Pane = wadView;
        }

        private async void OpenWADFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker opener = new FileOpenPicker(AppWindow.Id)
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeFilter = { ".wad" }
            };
            var result = await opener.PickSingleFileAsync();
            if (result is not null)
            {
                string path = result.Path;
                using (var stream = File.Open(result.Path, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        WAD wad = new WAD(reader, path);
                        wadView.Model.AddWAD(wad);
                    }
                }
            }
        }

        private void OpenGitHubFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/egartley/spyro-editor",
                UseShellExecute = true
            });
        }

        private void SettingsFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void AboutFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                XamlRoot = rootGrid.XamlRoot,
                Title = "Spyro Editor",
                Content = $"Version {Version.ToString()}",
                CloseButtonText = "Ok"
            };
            await dialog.ShowAsync();
        }

        private void ExitProgramFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
