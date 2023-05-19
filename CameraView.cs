#if __ANDROID__
using Directors_Viewfinder.Platforms.Android;
#endif

using Directors_Viewfinder.Interfaces;

namespace Directors_Viewfinder
{
    public class CameraView : View, ICameraView
    {
        public void TakePicture(string filepath)
        {
#if __ANDROID__
            if (Handler is CameraViewRenderer renderer)
            {
                renderer.Camera2View.TakePicture(filepath);
            }
#endif
        }
    }
}
