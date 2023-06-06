#if __ANDROID__
using Directors_Viewfinder.Platforms.Android.Camera;
#endif
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace Directors_Viewfinder
{
    public partial class MainPage : ContentPage
    {
#if __ANDROID__
        private CameraStateManager _cameraStateManager;
#endif
        private List<int> _focalLengths = new List<int> { 24, 35, 50, 85 };
        private int _currentFocalLengthIndex = 0;

        public MainPage()
        {
            InitializeComponent();
#if __ANDROID__
            _cameraStateManager = CameraStateManager.Instance;
#endif
            SwitchCameraButton.Clicked += SwitchCameraButton_Clicked;

            FocalLengthUpButton.Clicked += (s, e) => ChangeFocalLength(1);
            FocalLengthDownButton.Clicked += (s, e) => ChangeFocalLength(-1);

            UpdateFocalLengthLabel();
        }

        private void SwitchCameraButton_Clicked(object sender, EventArgs e)
        {
#if __ANDROID__
            _cameraStateManager.SwitchCamera();
#endif
        }

        private void ChangeFocalLength(int direction)
        {
            _currentFocalLengthIndex = Math.Max(0, Math.Min(_focalLengths.Count - 1, _currentFocalLengthIndex + direction));
            UpdateFocalLengthLabel();
        }

        private void UpdateFocalLengthLabel()
        {
            FocalLengthLabel.Text = $"{_focalLengths[_currentFocalLengthIndex]} mm";
        }
    }
}
