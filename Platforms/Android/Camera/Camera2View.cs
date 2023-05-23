using Android.Content;
using Android.Views;
using Android.Graphics;
using Android.Hardware.Camera2;
using static Android.Hardware.Camera2.CameraCaptureSession;
using static Android.Views.TextureView;
using Directors_Viewfinder.Android.Camera;
using RectF = Android.Graphics.RectF;
using Android.Runtime;

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

        public void ConfigureTransform(int viewWidth, int viewHeight)
        {
            if (_textureView == null || _textureView.SurfaceTexture == null)
            {
                return;
            }

            int rotation = (int)Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>().DefaultDisplay.Rotation;
            Matrix matrix = new();
            RectF viewRect = new(0, 0, viewWidth, viewHeight);
            RectF bufferRect = new(0, 0, viewHeight, viewWidth); // Assuming the camera sensor is in landscape orientation
            float centerX = viewRect.CenterX();
            float centerY = viewRect.CenterY();
            if (rotation == (int)SurfaceOrientation.Rotation90 || rotation == (int)SurfaceOrientation.Rotation270)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                float scale = Math.Max((float)viewHeight / viewWidth, (float)viewWidth / viewHeight);
                matrix.PostScale(scale, scale, centerX, centerY);
                matrix.PostRotate(90 * (rotation - 2), centerX, centerY);
            }
            else if (rotation == (int)SurfaceOrientation.Rotation180)
            {
                matrix.PostRotate(180, centerX, centerY);
            }
            _textureView.SetTransform(matrix);
        }

    }
}

