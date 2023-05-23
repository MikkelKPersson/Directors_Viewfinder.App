using Android.Graphics;
using Android.Views;
using Directors_Viewfinder.Platforms.Android.Camera;
using static Android.Views.TextureView;

namespace Directors_Viewfinder.Android.Camera
{
    public class SurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
    {
        private readonly Camera2View _camera2View;

        public SurfaceTextureListener(Camera2View camera2View)
        {
            _camera2View = camera2View;
        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            // The surface is ready, open the camera
            _camera2View.OpenCamera();
            _camera2View.ConfigureTransform(width, height);
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            _camera2View.ConfigureTransform(width, height);
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
        }


    }
}
