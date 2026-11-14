using HelixToolkit.WinUI;
using Microsoft.UI.Xaml.Input;

namespace Spyro_Editor.Rendering
{
    // dummy input controller to no-op helix's mouse input
    public class CameraInputController : InputController
    {
        public override void OnPointerPressed(PointerRoutedEventArgs e)
        {
        }

        public override void OnKeyPressed(KeyRoutedEventArgs e)
        {
        }
    }
}
