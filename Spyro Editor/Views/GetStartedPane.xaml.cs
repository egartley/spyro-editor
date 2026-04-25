using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Spyro_Editor.Views
{
    public sealed partial class GetStartedPane : Page
    {
        private string NoWADMessage;
        private string NoSubfileMessage;
        private MainWindow Main;

        public GetStartedPane(MainWindow main)
        {
            InitializeComponent();
            Main = main;
            NoWADMessage = "Select a WAD file to get started";
            NoSubfileMessage = "Select a subfile to view here";
            Message.Text = NoWADMessage;
        }

        public void OnWADLoaded()
        {
            OpenWADButton.Visibility = Visibility.Collapsed;
            Message.Text = NoSubfileMessage;
        }

        public void OnWADClosed()
        {
            OpenWADButton.Visibility = Visibility.Visible;
            Message.Text = NoWADMessage;
        }

        private void OpenWADButton_Click(object sender, RoutedEventArgs e)
        {
            Main.OpenWAD();
        }
    }
}
