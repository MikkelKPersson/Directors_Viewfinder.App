using System.Diagnostics;



namespace Directors_Viewfinder;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
        InitializeComponent();
    }

    private async void OnTakePictureClicked(object sender, EventArgs e)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permissions Denied", "Unable to take photos without Camera permission.", "OK");
                return;
            }
        }

        Debug.WriteLine("Taking picture...");
#if __ANDROID__
    await Task.Run(() => CameraView.TakePicture("test.jpg"));
#endif
        Debug.WriteLine("Picture taken.");
    }




    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                var result = await DisplayAlert("Permissions Denied", "Unable to take pictures without Camera permission. Would you like to enable permissions?", "Yes", "No");
                if (result)
                {
                    AppInfo.ShowSettingsUI();
                }
                return;
            }
        }

        status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                var result = await DisplayAlert("Permissions Denied", "Unable to save pictures without Storage permission. Would you like to enable permissions?", "Yes", "No");
                if (result)
                {
                    AppInfo.ShowSettingsUI();
                }
                return;
            }
        }
        Debug.WriteLine($"Camera permission status: {status}");
    }




}

