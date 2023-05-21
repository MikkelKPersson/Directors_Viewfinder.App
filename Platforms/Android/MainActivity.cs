using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace Directors_Viewfinder;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == 0)
        {
            for (int i = 0; i < permissions.Length; i++)
            {
                if (permissions[i] == Manifest.Permission.Camera)
                {
                    if (grantResults[i] == Permission.Granted)
                    {
                        // Camera permission was granted, start the camera preview
                    }
                    else
                    {
                        // Camera permission was denied, show a message to the user
                    }
                }
                else if (permissions[i] == Manifest.Permission.WriteExternalStorage)
                {
                    if (grantResults[i] == Permission.Granted)
                    {
                        // Storage permission was granted, save the picture
                    }
                    else
                    {
                        // Storage permission was denied, show a message to the user
                    }
                }
            }
        }
    }

    protected override void OnPause()
    {
        base.OnPause();

        // Log a message
        System.Diagnostics.Debug.WriteLine("App is going into the background");

        // Close the camera
        // You'll need to access your Camera2View instance here
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // Log a message
        System.Diagnostics.Debug.WriteLine("Activity is being destroyed");

        // Close the camera
        // You'll need to access your Camera2View instance here
    }

}
