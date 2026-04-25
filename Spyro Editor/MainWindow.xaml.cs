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
        private Version Version;
        private WADBrowser WADBrowser;
        private GetStartedPane GetStartedPane;
        private SubfilePane? SubfilePane;

        public MainWindow()
        {
            InitializeComponent();
            Version = Assembly.GetEntryAssembly()!.GetName().Version!;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(MainTitleBar);

            WADBrowser = new WADBrowser(this);
            GetStartedPane = new GetStartedPane(this);

            MainSplitView.Pane = WADBrowser;
            MainSplitView.Content = GetStartedPane;
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
                GetStartedPane.OnWADLoaded();
            }
        }

        private void CloseWAD()
        {
            if (SubfilePane is not null)
            {
                SubfilePane.Close();
                SubfilePane.Visibility = Visibility.Collapsed;
            }
            WADBrowser.Unload();
            GetStartedPane.OnWADClosed();
            GetStartedPane.Visibility = Visibility.Visible;
            MainSplitView.Content = GetStartedPane;
        }

        public void LoadSubfile(string wadPath, Subfile subfile)
        {
            if (SubfilePane is null)
            {
                SubfilePane = new SubfilePane();
            }
            SubfilePane.Load(wadPath, subfile);

            GetStartedPane.Visibility = Visibility.Collapsed;
            SubfilePane.Visibility = Visibility.Visible;
            MainSplitView.Content = SubfilePane;
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

        private void ExitFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
