using Android.Hardware.Camera2;
using Android.Views;

namespace Directors_Viewfinder.Android.Camera
{
    public class CameraStateManager
    {
        private CameraDevice _cameraDevice;
        private CameraCaptureSession _captureSession;

        public CameraDevice CameraDevice
        {
            get { return _cameraDevice; }
            set { _cameraDevice = value; }
        }

        public CameraCaptureSession CaptureSession
        {
            get { return _captureSession; }
            set { _captureSession = value; }
        }

        public void OpenCamera(CameraManager cameraManager, string cameraId, CameraDevice.StateCallback stateCallback, Handler handler)
        {
            cameraManager.OpenCamera(cameraId, stateCallback, handler);
        }

        public void CloseCamera()
        {
            _captureSession?.Close();
            _captureSession = null;
            _cameraDevice?.Close();
            _cameraDevice = null;
        }

        public void CreateCaptureSession(CameraDevice cameraDevice, Surface surface, CameraCaptureSession.StateCallback stateCallback, Handler handler)
        {
            cameraDevice.CreateCaptureSession(new List<Surface>() { surface }, stateCallback, handler);
        }

        public void DisposeCaptureSession()
        {
            _captureSession?.Dispose();
            _captureSession = null;
        }
    }
}
