#if __ANDROID__
using Directors_Viewfinder.Platforms.Android.Camera;
#endif
namespace Directors_Viewfinder
{
    public partial class MainPage : ContentPage
    {
#if __ANDROID__
        private CameraStateManager _cameraStateManager;

#endif

        public MainPage()
        {
            InitializeComponent();
#if __ANDROID__
            _cameraStateManager = CameraStateManager.Instance;
#endif
            SwitchCameraButton.Clicked += SwitchCameraButton_Clicked;
        }

        private void SwitchCameraButton_Clicked(object sender, EventArgs e)
        {
#if __ANDROID__
            _cameraStateManager.SwitchCamera();
#endif
        }
    }
}
