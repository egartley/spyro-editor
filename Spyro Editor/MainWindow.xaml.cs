using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Diagnostics;

namespace Spyro_Editor
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(mainTitleBar);
        }

        private async void OpenWADFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker opener = new FileOpenPicker(AppWindow.Id)
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeFilter = { ".wad" }
            };
            var file = await opener.PickSingleFileAsync();
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
                Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce eget velit quis eros interdum venenatis at sed enim.",
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
