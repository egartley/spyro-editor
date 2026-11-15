using HelixToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SharpDX;
using Spyro_Editor.Contexts;
using Spyro_Editor.Models;
using Spyro_Editor.Rendering;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.System;
using Windows.UI.Core;

namespace Spyro_Editor.Views
{
    public sealed partial class HelixViewer : Page
    {
        private const float SceneMoveSpeedMult = 8.0f / 60.0f;
        private const float KeyMoveFastMult = 5.0f;
        private const float LinearMoveAcceleration = 1.0f / 5.0f;
        private const float KeyMoveDrag = 0.8f;
        private const float KeyMoveThreshold = 0.1f;
        private const float MouseLookSpeed = 500.0f;
        private readonly HelixModel Model;
        private readonly Stopwatch MoveTimer = new();
        private IntPtr WindowHandle;
        private float KeyMoveSpeed = 60.0f;
        private Vector3 LinearMoveDir;
        private Vector2 MouseDelta;
        private Windows.Foundation.Point? LastPointerPosition;
        private bool AreBothButtonsPressed;
        private bool IsClosed;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public HelixViewer()
        {
            InitializeComponent();
            Model = new HelixModel();

            Viewport.Camera = new PerspectiveCamera() { FarPlaneDistance = 1e6, NearPlaneDistance = 10 };
            Viewport.EffectsManager = new PS1EffectsManager();
            Viewport.InputController = new CameraInputController();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SubfileContext context)
            {
                WindowHandle = Win32Interop.GetWindowFromWindowId(context.WindowId);
                await Model.Load(context.Subfile.Level!, context.Subfile.Game);
                if (IsClosed)
                {
                    // navigated away mid-load
                    Model.Close();
                    return;
                }
                Viewport.Camera.Position = Model.Centroid;
                PartsPaneTitle.Text = $"Parts ({Model.VisibleMeshes.Count})";
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            IsClosed = true;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            Model.Close();
            Viewport.EffectsManager.Dispose();
            base.OnNavigatedFrom(e);
        }

        private static bool IsKeyDown(VirtualKey key)
        {
            return InputKeyboardSource.GetKeyStateForCurrentThread(key).HasFlag(CoreVirtualKeyStates.Down);
        }

        private static float UpdateMoveAxis(float value, bool negative, bool positive, float velocity, float cap)
        {
            if (negative)
            {
                return Math.Clamp(value - velocity, -cap, cap);
            }
            if (positive)
            {
                return Math.Clamp(value + velocity, -cap, cap);
            }
            if (Math.Abs(value) >= KeyMoveThreshold)
            {
                value *= KeyMoveDrag;
                if (Math.Abs(value) < KeyMoveThreshold)
                {
                    value = 0.0f;
                }
            }
            return value;
        }

        private void Viewport_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(Viewport);
            Viewport.CapturePointer(e.Pointer);
            LastPointerPosition = point.Position;
            AreBothButtonsPressed = point.Properties.IsLeftButtonPressed && point.Properties.IsRightButtonPressed;
        }

        private void Viewport_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (LastPointerPosition is not Windows.Foundation.Point last)
            {
                return;
            }
            PointerPoint point = e.GetCurrentPoint(Viewport);
            MouseDelta.X += (float)(point.Position.X - last.X);
            MouseDelta.Y += (float)(point.Position.Y - last.Y);
            LastPointerPosition = point.Position;
            AreBothButtonsPressed = point.Properties.IsLeftButtonPressed && point.Properties.IsRightButtonPressed;
        }

        private void Viewport_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(Viewport);
            AreBothButtonsPressed = false;
            if (!point.Properties.IsLeftButtonPressed && !point.Properties.IsRightButtonPressed && !point.Properties.IsMiddleButtonPressed)
            {
                Viewport.ReleasePointerCapture(e.Pointer);
                LastPointerPosition = null;
            }
        }

        private void Viewport_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            AreBothButtonsPressed = false;
            LastPointerPosition = null;
        }

        private void Viewport_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            int delta = e.GetCurrentPoint(Viewport).Properties.MouseWheelDelta;
            KeyMoveSpeed = Math.Clamp(KeyMoveSpeed + Math.Sign(delta) * 4.0f, 1.0f, 200.0f);
        }

        private void CompositionTarget_Rendering(object? sender, object e)
        {
            float frames = (float)MoveTimer.Elapsed.TotalSeconds * 60.0f;
            MoveTimer.Restart();
            bool isActive = GetForegroundWindow() == WindowHandle;
            bool isFast = isActive && IsKeyDown(VirtualKey.Shift);
            float cap = KeyMoveSpeed * (isFast ? KeyMoveFastMult : 1.0f);
            float velocity = cap * LinearMoveAcceleration;
            LinearMoveDir.Z = UpdateMoveAxis(LinearMoveDir.Z,
                isActive && (IsKeyDown(VirtualKey.S) || IsKeyDown(VirtualKey.Down)),
                isActive && (IsKeyDown(VirtualKey.W) || IsKeyDown(VirtualKey.Up) || AreBothButtonsPressed),
                velocity, cap);
            LinearMoveDir.X = UpdateMoveAxis(LinearMoveDir.X,
                isActive && (IsKeyDown(VirtualKey.A) || IsKeyDown(VirtualKey.Left)),
                isActive && (IsKeyDown(VirtualKey.D) || IsKeyDown(VirtualKey.Right)),
                velocity, cap);
            LinearMoveDir.Y = UpdateMoveAxis(LinearMoveDir.Y,
                isActive && IsKeyDown(VirtualKey.Q),
                isActive && IsKeyDown(VirtualKey.E),
                velocity, cap);

            // camera logic borrowed from noclip.website
            Camera camera = Viewport.Camera;
            Vector3 forward = Vector3.Normalize(camera.LookDirection);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, camera.UpDirection));
            if (LinearMoveDir != Vector3.Zero)
            {
                Vector3 velocityVector = right * LinearMoveDir.X + forward * LinearMoveDir.Z + Vector3.UnitY * LinearMoveDir.Y;
                camera.Position += velocityVector * SceneMoveSpeedMult * Math.Min(frames, 3.0f);
            }

            float yaw = -MouseDelta.X / MouseLookSpeed;
            float pitch = -MouseDelta.Y / MouseLookSpeed;
            MouseDelta = Vector2.Zero;
            if (yaw != 0.0f || pitch != 0.0f)
            {
                SharpDX.Matrix rotation = SharpDX.Matrix.RotationAxis(right, pitch) * SharpDX.Matrix.RotationAxis(Vector3.UnitY, yaw);
                camera.LookDirection = Vector3.TransformNormal(camera.LookDirection, rotation);
                camera.UpDirection = Vector3.TransformNormal(camera.UpDirection, rotation);
            }
        }

        private void PartListItemCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.DataContext is PartListItem part)
                {
                    Model.UpdateMeshVisibility(part.Id, part.IsVisible);
                }
            }
        }

        private void ShowAllPartsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(PartListItem item in Model.ListItems)
            {
                item.IsVisible = true;
                Model.UpdateMeshVisibility(item.Id, true);
            }
        }

        private void HideAllPartsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (PartListItem item in Model.ListItems)
            {
                item.IsVisible = false;
                Model.UpdateMeshVisibility(item.Id, false);
            }
        }
    }
}
