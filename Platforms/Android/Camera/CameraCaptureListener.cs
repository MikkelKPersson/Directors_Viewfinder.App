using Android.Hardware.Camera2;
using Directors_Viewfinder.Platforms.Android.Camera;

namespace Directors_Viewfinder.Android.Camera
{
    public class CameraCaptureListener : CameraCaptureSession.CaptureCallback
    {
        private readonly Camera2View _camera2View;

        public CameraCaptureListener(Camera2View camera2View)
        {
            _camera2View = camera2View;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            // Handle the capture completed event
            // This method is called when the capture request is completed.
            // You can update your UI or perform other tasks here.
            _camera2View.Process(result);
        }

        public override void OnCaptureFailed(CameraCaptureSession session, CaptureRequest request, CaptureFailure failure)
        {
            // Handle the capture failed event
            // This method is called when the capture request fails.
            // You can update your UI or perform other tasks here.
            System.Diagnostics.Debug.WriteLine("Failed to capture image!");
        }
    }
}
