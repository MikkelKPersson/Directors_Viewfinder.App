using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;

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
}
