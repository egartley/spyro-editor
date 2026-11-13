namespace Spyro_Editor.Contexts
{
    public class GetStartedContext
    {
        public bool IsWADOpen;
        public MainWindow MainWindow;

        public GetStartedContext(MainWindow mainWindow, bool isWADOpen)
        {
            MainWindow = mainWindow;
            IsWADOpen = isWADOpen;
        }
    }
}
