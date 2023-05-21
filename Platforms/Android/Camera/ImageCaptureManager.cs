using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Media;
using Android.Views;

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

        public void TakePicture(CameraDevice cameraDevice, Surface surface, CaptureRequest.Builder requestBuilder, CameraCaptureSession.CaptureCallback captureCallback, Handler handler)
        {
            requestBuilder.AddTarget(surface);
            cameraDevice.CreateCaptureSession(new List<Surface>() { surface }, new CameraCaptureSessionCallback(), handler);
        }

        public void DisposeImageReader()
        {
            _imageReader?.Dispose();
            _imageReader = null;
        }
    }
}
