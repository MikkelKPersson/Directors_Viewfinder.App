using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Directors_Viewfinder.Platforms.Android;
using Directors_Viewfinder;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Platform;

[assembly: ExportRenderer(typeof(CameraView), typeof(CameraViewRenderer))]
namespace Directors_Viewfinder.Platforms.Android
{
    public class CameraViewRenderer : ViewRenderer<CameraView, Camera2View>
    {
        public CameraViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CameraView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                // Initialize the native view and set it as the control's native view
                var camera2View = new Camera2View(Context);
                SetNativeControl(camera2View);
            }
        }
    }
}
