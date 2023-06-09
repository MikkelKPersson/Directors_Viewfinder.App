﻿using Directors_Viewfinder.Platforms.Android;
using Directors_Viewfinder;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Handlers;
using Directors_Viewfinder.Platforms.Android.Camera;

[assembly: ExportRenderer(typeof(CameraView), typeof(CameraViewRenderer))]
namespace Directors_Viewfinder.Platforms.Android.Camera
{
    public class CameraViewRenderer : ViewHandler<CameraView, Camera2View>
    {
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
            // Additional setup when the native view is connected to the handler
        }

        protected override void DisconnectHandler(Camera2View nativeView)
        {
            base.DisconnectHandler(nativeView);
            // Additional cleanup when the native view is disconnected from the handler
        }
    }
}
