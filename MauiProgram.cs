using Microsoft.Extensions.Logging;
<<<<<<< HEAD
#if __ANDROID__
using Directors_Viewfinder.Platforms.Android;
#endif
=======
>>>>>>> parent of 9299287 (App working with camera2 camera view)

namespace Directors_Viewfinder;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
<<<<<<< HEAD
            .ConfigureMauiHandlers(handlers =>
            {
#if __ANDROID__
                handlers.AddHandler<CameraView, CameraViewRenderer>();
#endif
			})
            .ConfigureFonts(fonts =>
=======
			.ConfigureFonts(fonts =>
>>>>>>> parent of 9299287 (App working with camera2 camera view)
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
