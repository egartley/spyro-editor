using Microsoft.UI.Xaml;
using System;

namespace Spyro_Editor
{
    public partial class App : Application
    {
        public static Version Version = new Version(0, 0, 0);
        private Window? _window;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
