#if __ANDROID__
using Directors_Viewfinder.Platforms.Android;
#endif

namespace Directors_Viewfinder
{
    public class CameraView : View
    {
        // You can add properties and methods here that you want to expose to the shared project
#if __ANDROID__
        public void TakePicture(string filepath)
        {
            Task.Run(() =>
            {
                (Handler as CameraViewRenderer)?.Camera2View.TakePicture(filepath);
            });
        }
#endif
    }
}
