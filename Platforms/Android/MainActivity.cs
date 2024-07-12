using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Directors_Viewfinder.Platforms.Android.Camera;
using View = Android.Views.View;

namespace Directors_Viewfinder;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, WindowSoftInputMode = SoftInput.AdjustPan)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        RequestedOrientation = ScreenOrientation.Landscape;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            // For Android 11 (API level 30) and later
            Window.SetDecorFitsSystemWindows(false);
            Window.InsetsController?.Hide(WindowInsets.Type.StatusBars() | WindowInsets.Type.NavigationBars());
        }
        else
        {
            // For earlier versions
            View decorView = Window.DecorView;
            var uiOptions = (int)decorView.SystemUiVisibility;
            var newUiOptions = (int)uiOptions;

            newUiOptions |= (int)SystemUiFlags.LowProfile;
            newUiOptions |= (int)SystemUiFlags.Fullscreen;
            newUiOptions |= (int)SystemUiFlags.HideNavigation;
            newUiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            decorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

        }
        if (CheckSelfPermission(Manifest.Permission.Camera) != Permission.Granted)
        {
            RequestPermissions(new string[] { Manifest.Permission.Camera }, 0);
        }

        // Obtain the CameraManager from the system service
        CameraManager cameraManager = (CameraManager)GetSystemService(Context.CameraService);

        // Pass the CameraManager to the CameraUtilities constructor
        CameraUtilities cameraUtilities = new CameraUtilities(cameraManager);

        CameraAvailabilityCallback availabilityCallback = new CameraAvailabilityCallback();

        availabilityCallback.CameraAvailable += (sender, cameraId) =>
        {
            // Camera is available, react accordingly
        };

        availabilityCallback.CameraUnavailable += (sender, cameraId) =>
        {
            // Camera is unavailable, react accordingly
        };

        cameraUtilities.RegisterAvailabilityCallback(availabilityCallback);


    }

    protected override void OnResume()
    {
        base.OnResume();

        if (CheckSelfPermission(Manifest.Permission.Camera) != Permission.Granted)
        {
            RequestPermissions(new string[] { Manifest.Permission.Camera }, 0);
        }
    }
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == 0)
        {
            if (grantResults[0] == Permission.Granted)
            {
                // Permission was granted, start the camera preview
            }
            else
            {
                // Permission was denied, show a message to the user
            }
        }
    }

}
