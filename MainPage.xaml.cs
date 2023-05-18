namespace Directors_Viewfinder;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
        InitializeComponent();
    }

    private void OnTakePictureClicked(object sender, EventArgs e)
    {
        var filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "test.jpg");
#if __ANDROID__
        Task.Run(() => CameraView.TakePicture(filepath));
#endif
    }

}

