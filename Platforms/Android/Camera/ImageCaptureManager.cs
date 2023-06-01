using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Media;
using Android.Views;
using Android.OS;
using Android.Hardware.Camera2.Params;
using Directors_Viewfinder.Platforms.Android.Camera;

namespace Directors_Viewfinder.Android.Camera
{
    public class ImageCaptureManager
    {
        private ImageReader _imageReader;

        public ImageReader ImageReader
        {
            get { return _imageReader; }
            set { _imageReader = value; }
        }

        public void SetUpImageReader(int width, int height, ImageFormatType format, int maxImages)
        {
            _imageReader = ImageReader.NewInstance(width, height, format, maxImages);
        }

        public static void TakePicture(CameraDevice cameraDevice, Surface surface, CaptureRequest.Builder requestBuilder, Handler handler)
        {
            requestBuilder.AddTarget(surface);
            OutputConfiguration outputConfiguration = new(surface);
            IList<OutputConfiguration> configurations = new List<OutputConfiguration>() { outputConfiguration };
            SessionConfiguration sessionConfiguration = new((int)SessionType.Regular, configurations, new HandlerExecutor(handler), new ImageCaptureSessionCallback());
            cameraDevice.CreateCaptureSession(sessionConfiguration);
        }







        public void DisposeImageReader()
        {
            _imageReader?.Dispose();
            _imageReader = null;
        }
    }

    public class SimpleCaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        public override void OnConfigured(CameraCaptureSession session)
        {
            // The camera is already closed
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            // Show toast message if configuration changes failed
        }
    }

    public class ImageCaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        // Override methods as needed
        public override void OnConfigured(CameraCaptureSession session)
        {
            // Implementation here
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            // Implementation here
        }
    }


}
