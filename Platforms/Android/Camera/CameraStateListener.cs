using Android.Hardware.Camera2;
using Directors_Viewfinder.Platforms.Android.Camera;

namespace Directors_Viewfinder.Android.Camera
{
    public class CameraStateListener : CameraDevice.StateCallback
    {
        private readonly Camera2View _camera2View;

        public CameraStateListener(Camera2View camera2View)
        {
            _camera2View = camera2View;
        }

        public override void OnOpened(CameraDevice camera)
        {
            // Handle the camera opened event
            // This method is called when the camera device is opened.
            // You can update your UI or perform other tasks here.
            _camera2View.CameraOpen(camera);
        }

        public override void OnDisconnected(CameraDevice camera)
        {
            // Handle the camera disconnected event
            // This method is called when the camera device is disconnected.
            // You can update your UI or perform other tasks here.
            _camera2View.CameraClose();
        }

        public override void OnError(CameraDevice camera, CameraError error)
        {
            // Handle the camera error event
            // This method is called when an error occurs with the camera device.
            // You can update your UI or perform other tasks here.
            _camera2View.CameraClose();
        }
    }
}
