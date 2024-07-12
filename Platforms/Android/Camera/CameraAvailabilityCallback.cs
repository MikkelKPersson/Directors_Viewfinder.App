using Android.Hardware.Camera2;
using System;

namespace Directors_Viewfinder.Platforms.Android.Camera
{
    public class CameraAvailabilityCallback : CameraManager.AvailabilityCallback
    {
        public event EventHandler<string> CameraAvailable;
        public event EventHandler<string> CameraUnavailable;

        public override void OnCameraAvailable(string cameraId)
        {
            // Notify subscribers that the camera is now available
            CameraAvailable?.Invoke(this, cameraId);
        }

        public override void OnCameraUnavailable(string cameraId)
        {
            // Notify subscribers that the camera is now unavailable
            CameraUnavailable?.Invoke(this, cameraId);
        }
    }
}