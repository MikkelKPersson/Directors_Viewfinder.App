using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace Directors_Viewfinder;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
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
