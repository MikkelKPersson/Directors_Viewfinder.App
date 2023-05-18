using Directors_Viewfinder.Platforms.Android;
using Directors_Viewfinder;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Handlers;

[assembly: ExportRenderer(typeof(CameraView), typeof(CameraViewRenderer))]
namespace Directors_Viewfinder.Platforms.Android
{
    public class CameraViewRenderer : ViewHandler<CameraView, Camera2View>
    {
        private Camera2View _camera2View;

        public CameraViewRenderer() : base(Microsoft.Maui.Handlers.ViewHandler.ViewMapper)
        {
        }

        protected override Camera2View CreatePlatformView()
        {
            return new Camera2View(Context);
        }

        protected override void ConnectHandler(Camera2View nativeView)
        {
            base.ConnectHandler(nativeView);
            _camera2View = nativeView;
            // Additional setup when the native view is connected to the handler
        }

        protected override void DisconnectHandler(Camera2View nativeView)
        {
            base.DisconnectHandler(nativeView);
            _camera2View = null;
            // Additional cleanup when the native view is disconnected from the handler
        }

        // Expose the Camera2View instance
        public Camera2View Camera2View => _camera2View;
    }
}

