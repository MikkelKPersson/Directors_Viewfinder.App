using Android.Content;

using Android.Views;
using Android.Graphics;
using Android.Hardware.Camera2;
using static Android.Hardware.Camera2.CameraCaptureSession;
using static Android.Views.TextureView;
using Android.Media;
using Android.Provider;
using Android.App;
using Application = Android.App.Application;
using Android.Hardware.Camera2.Params;
using Android.OS;
using Java.Lang;
using Java.Util.Concurrent;
using Bumptech.Glide.Util;
using NativeMedia;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;

namespace Directors_Viewfinder.Platforms.Android
{
    public class Camera2View : ViewGroup
    {
        private readonly CameraManager _cameraManager;
        private readonly string _cameraId;

        private CameraDevice _cameraDevice;
        private CameraCaptureSession _captureSession;

        private CaptureRequest.Builder _previewRequestBuilder;

        private readonly TextureView _textureView;



        public Camera2View(Context context) : base(context)
        {
            _cameraManager = (CameraManager)context.GetSystemService(Context.CameraService);
            _cameraId = _cameraManager.GetCameraIdList()[0]; // Get the first back-facing camera

            _textureView = new TextureView(context);
            AddView(_textureView);

            _textureView.SurfaceTextureListener = new SurfaceTextureListener(this);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            _textureView.Layout(0, 0, r - l, b - t);
        }

        public void OpenCamera()
        {
            // Open the camera
            _cameraManager.OpenCamera(_cameraId, new CameraStateCallback(this), null);
        }

        public void CloseCamera()
        {
            // Close the camera and stop the preview
            System.Diagnostics.Debug.WriteLine("CloseCamera() called");
            _captureSession?.Close();
            _captureSession = null;

            _cameraDevice?.Close();
            _cameraDevice = null;
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();

            // Close the camera when the view is detached from the window
            CloseCamera();
        }

        private class CameraStateCallback : CameraDevice.StateCallback
        {
            private readonly Camera2View _camera2View;

            public CameraStateCallback(Camera2View camera2View)
            {
                _camera2View = camera2View;
            }

            public override void OnOpened(CameraDevice camera)
            {
                // Camera opened successfully. Now we can start the preview
                _camera2View._cameraDevice = camera;

                // Create a CaptureRequest.Builder for the preview
                _camera2View._previewRequestBuilder = camera.CreateCaptureRequest(CameraTemplate.Preview);

                // Set the TextureView's Surface as the target of the CaptureRequest.Builder
                Surface surface = new(_camera2View._textureView.SurfaceTexture);
                _camera2View._previewRequestBuilder.AddTarget(surface);

                // Create a CameraCaptureSession for the preview
                List<OutputConfiguration> outputConfigurations = new List<OutputConfiguration> { new OutputConfiguration(surface) };
                IExecutor mainExecutor = new HandlerExecutor(new Handler(Looper.MainLooper));
                SessionConfiguration sessionConfiguration = new SessionConfiguration((int)SessionType.Regular,
                    outputConfigurations, mainExecutor, new CaptureStateCallback(_camera2View));

                if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                {
                    // Android 9.0 Pie (API level 28) and later
                    camera.CreateCaptureSession(sessionConfiguration);
                }
                else
                {
                    // Alternative method for earlier Android versions
                }

            }

            public override void OnDisconnected(CameraDevice camera)
            {
                // Camera has been disconnected
                _camera2View.CloseCamera();
            }

            public override void OnError(CameraDevice camera, CameraError error)
            {
                System.Diagnostics.Debug.WriteLine($"OnError() called with error: {error}");
                _camera2View.CloseCamera();
            }

        }

        private class CaptureStateCallback : CameraCaptureSession.StateCallback
        {
            private readonly Camera2View _camera2View;

            public CaptureStateCallback(Camera2View camera2View)
            {
                _camera2View = camera2View;
            }

            public override void OnConfigured(CameraCaptureSession session)
            {
                System.Diagnostics.Debug.WriteLine("Session configured...");
                // The camera is already closed
                if (_camera2View._cameraDevice == null)
                {
                    return;
                }

                // When the session is ready, we start displaying the preview
                _camera2View._captureSession = session;
                try
                {
                    // Auto focus should be continuous for camera preview
                    _camera2View._previewRequestBuilder.Set(CaptureRequest.ControlAfMode,
                        (int)ControlAFMode.ContinuousPicture);

                    // Finally, we start displaying the camera preview
                    CaptureRequest previewRequest = _camera2View._previewRequestBuilder.Build();
                    _camera2View._captureSession.SetRepeatingRequest(previewRequest, new CaptureCallback(), null);
                }
                catch (CameraAccessException)
                {
                    // Log the exception
                }
            }

            public override void OnConfigureFailed(CameraCaptureSession session)
            {
                // Show toast message if configuration changes failed
            }
        }

        private class CaptureCallback : CameraCaptureSession.CaptureCallback
        {
            public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
            {
                // You can override other methods here if needed
            }
        }

        public class HandlerExecutor : Java.Lang.Object, IExecutor
        {
            private readonly Handler handler;

            public HandlerExecutor(Handler handler)
            {
                this.handler = handler;
            }

            public void Execute(IRunnable command)
            {
                handler.Post(command);
            }
        }

        private class SurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
        {
            private readonly Camera2View _camera2View;

            public SurfaceTextureListener(Camera2View camera2View)
            {
                _camera2View = camera2View;
            }

            public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
            {
                // The surface is ready, open the camera
                _camera2View.OpenCamera();
            }

            public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
            {
                return true;
            }

            public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
            {
            }

            public void OnSurfaceTextureUpdated(SurfaceTexture surface)
            {
            }
        }

        public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
        {
            public Action<ImageReader> OnImageAvailableAction { get; set; }

            public void OnImageAvailable(ImageReader reader)
            {
                OnImageAvailableAction?.Invoke(reader);
            }
        }

        public class CameraCaptureListener : CameraCaptureSession.CaptureCallback
        {
            public Action<CaptureResult> OnCaptureCompletedAction { get; set; }

            public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
            {
                OnCaptureCompletedAction?.Invoke(result);
            }
        }





        public async void TakePicture(string filename)
        {
            System.Diagnostics.Debug.WriteLine("TakePicture() called");
            // Create a ImageReader object for images of the desired size
            var reader = ImageReader.NewInstance(640, 480, ImageFormatType.Jpeg, 1);

            // Create a new capture request
            var captureBuilder = _cameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);

            // Set the ImageReader's surface as the target of the capture request
            Surface readerSurface = reader.Surface;
            captureBuilder.AddTarget(readerSurface);

            // Set up a callback to receive the image data when the image is available
            var readerListener = new ImageAvailableListener()
            {
                OnImageAvailableAction = async (ImageReader r) =>
                {
                    System.Diagnostics.Debug.WriteLine("Image available...");
                    // Get the first image in the queue
                    using var image = r.AcquireNextImage();

                    // Convert the image to byte array
                    var buffer = image.GetPlanes()[0].Buffer;
                    var data = new byte[buffer.Remaining()];
                    buffer.Get(data);

                    await MediaGallery.SaveAsync(MediaFileType.Image, data, "Directors Viewfinder");
                    System.Diagnostics.Debug.WriteLine("Image saved.");
                }
            };

            // Create a new HandlerThread and start it
            var handlerThread = new HandlerThread("CameraPicture");
            handlerThread.Start();

            // Create a Handler associated with the HandlerThread's Looper
            var handler = new Handler(handlerThread.Looper);

            // Set the callback to the ImageReader
            Device.BeginInvokeOnMainThread(() =>
            {
                reader.SetOnImageAvailableListener(readerListener, handler);
            });

            // Create a list of surfaces
            List<Surface> surfaces = new List<Surface>();
            surfaces.Add(readerSurface);

            // Create a CameraCaptureSession for the preview
            _cameraDevice.CreateCaptureSession(surfaces, new CaptureStateCallback(this), handler);

            // Capture the image
            System.Diagnostics.Debug.WriteLine("Capturing image...");
            _captureSession.Capture(captureBuilder.Build(), new CameraCaptureListener(), handler);
        }











        public async Task SaveImageToGalleryAsync(byte[] imageBytes, string filename)
        {
            var contentValues = new ContentValues();
            contentValues.Put(MediaStore.Images.Media.InterfaceConsts.DisplayName, filename);
            contentValues.Put(MediaStore.Images.Media.InterfaceConsts.MimeType, "image/jpeg");

            var uri = Application.Context.ContentResolver.Insert(MediaStore.Images.Media.ExternalContentUri, contentValues);

            using var stream = Application.Context.ContentResolver.OpenOutputStream(uri);
            await stream.WriteAsync(imageBytes, 0, imageBytes.Length);
        }
    }
}
