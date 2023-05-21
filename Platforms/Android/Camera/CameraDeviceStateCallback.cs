using Android.Hardware.Camera2;
using Directors_Viewfinder.Platforms.Android.Camera;

namespace Directors_Viewfinder.Droid.Camera
{
    public class CameraDeviceStateCallback : CameraDevice.StateCallback
    {
        private readonly Camera2View _camera2View;

        public CameraDeviceStateCallback(Camera2View camera2View)
        {
            _camera2View = camera2View;
        }

        public override void OnOpened(CameraDevice camera)
        {
            // This method is called when the camera is opened. We start camera preview here.
            _camera2View._cameraDevice = camera;
            _camera2View.CreateCameraPreviewSession();
        }

        public override void OnDisconnected(CameraDevice camera)
        {
            _camera2View._cameraDevice.Close();
            _camera2View._cameraDevice = null;
        }

        public override void OnError(CameraDevice camera, CameraError error)
        {
            _camera2View._cameraDevice.Close();
            _camera2View._cameraDevice = null;
            // TODO: Handle the error appropriately in your app
        }
    }
}
