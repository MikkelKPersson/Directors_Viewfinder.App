using Android.Hardware.Camera2;
using Directors_Viewfinder.Platforms.Android.Camera;

namespace Directors_Viewfinder.Droid.Camera
{
    public class CameraCaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        private readonly Camera2View _camera2View;

        public CameraCaptureSessionCallback(Camera2View camera2View)
        {
            _camera2View = camera2View;
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            // The camera is already closed
            if (_camera2View._cameraDevice == null)
                return;

            // When the session is ready, we start displaying the preview.
            _camera2View._captureSession = session;
            _camera2View.StartPreview();
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            System.Diagnostics.Debug.WriteLine("Failed to configure camera.");
        }
    }
}
