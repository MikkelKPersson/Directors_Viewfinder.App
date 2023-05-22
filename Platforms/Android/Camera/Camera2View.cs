using Android.Content;
using Android.Views;
using Android.Graphics;
using Android.Hardware.Camera2;
using static Android.Hardware.Camera2.CameraCaptureSession;
using static Android.Views.TextureView;
using Directors_Viewfinder.Android.Camera;

namespace Directors_Viewfinder.Platforms.Android.Camera
{
    public class Camera2View : ViewGroup
    {
        private readonly CameraManager _cameraManager;
        private readonly string _cameraId;

        private readonly CameraStateManager _cameraStateManager = new();

        private readonly TextureView _textureView;

        public Camera2View(Context context) : base(context)
        {
            _cameraManager = (CameraManager)context.GetSystemService(Context.CameraService);
            _cameraId = _cameraManager.GetCameraIdList()[0]; // Get the first back-facing camera
            _textureView = new TextureView(context);
            AddView(_textureView);

            _textureView.SurfaceTextureListener = new SurfaceTextureListener(this);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            _textureView.Layout(0, 0, r - l, b - t);
        }

        public void OpenCamera()
        {
            // Open the camera
            CameraStateManager.OpenCamera(_cameraManager, _cameraId, new CameraStateCallback(_cameraStateManager, _textureView.SurfaceTexture), null);
        }


        public void CloseCamera()
        {
            // Close the camera and stop the preview
            _cameraStateManager.CloseCamera();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();

            // Close the camera when the view is detached from the window
            CloseCamera();
        }
    }
}

