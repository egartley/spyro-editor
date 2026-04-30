using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spyro_Editor.Contexts;

namespace Spyro_Editor.Views
{
    public sealed partial class GetStartedPane : Page
    {
        private string NoWADMessage;
        private string NoSubfileMessage;
        private MainWindow? Main;

        public GetStartedPane()
        {
            InitializeComponent();
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is GetStartedContext context)
            {
                Main = context.MainWindow;
                if (context.IsWADOpen)
                {
                    OnWADLoaded();
                }
                else
                {
                    OnWADClosed();
                }
            }
            base.OnNavigatedTo(e);
        }

        private void OpenWADButton_Click(object sender, RoutedEventArgs e)
        {
            Main!.OpenWAD();
        }
    }
}
