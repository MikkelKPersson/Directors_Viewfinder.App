    using Android.Content;
    using Android.Views;
    using Android.Graphics;
    using Android.Hardware.Camera2;
    using static Android.Hardware.Camera2.CameraCaptureSession;
    using static Android.Views.TextureView;
    using Android.Media;

    namespace Directors_Viewfinder.Platforms.Android
    {
        public class Camera2View : ViewGroup
        {
            private CameraManager _cameraManager;
            private string _cameraId;

            private CameraDevice _cameraDevice;
            private CameraCaptureSession _captureSession;

            private CaptureRequest.Builder _previewRequestBuilder;

            private TextureView _textureView;



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
                private Camera2View _camera2View;

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
                    Surface surface = new Surface(_camera2View._textureView.SurfaceTexture);
                    _camera2View._previewRequestBuilder.AddTarget(surface);

                    // Create a CameraCaptureSession for the preview
                    camera.CreateCaptureSession(new List<Surface>() { surface }, new CaptureStateCallback(_camera2View), null);
                }

                public override void OnDisconnected(CameraDevice camera)
                {
                    // Camera has been disconnected
                    _camera2View.CloseCamera();
                }

                public override void OnError(CameraDevice camera, CameraError error)
                {
                    // An error occurred while opening the camera
                    _camera2View.CloseCamera();
                }
            }

            private class CaptureStateCallback : CameraCaptureSession.StateCallback
            {
                private Camera2View _camera2View;

                public CaptureStateCallback(Camera2View camera2View)
                {
                    _camera2View = camera2View;
                }

                public override void OnConfigured(CameraCaptureSession session)
                {
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

            private class SurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
            {
                private Camera2View _camera2View;

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





        public void TakePicture(string filepath)
        {
            // Create a ImageReader object for images of the desired size
            var reader = ImageReader.NewInstance(640, 480, ImageFormatType.Jpeg, 1);

            // Create a new capture request
            var captureBuilder = _cameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);

            // Set the ImageReader's surface as the target of the capture request
            captureBuilder.AddTarget(reader.Surface);

            // Set up a callback to receive the image data when the image is available
            var readerListener = new ImageAvailableListener()
            {
                OnImageAvailableAction = (ImageReader r) =>
                {
                    // Get the first image in the queue
                    using var image = r.AcquireNextImage();

                    // Convert the image to byte array
                    var buffer = image.GetPlanes()[0].Buffer;
                    var data = new byte[buffer.Remaining()];
                    buffer.Get(data);

                    // Save the image to a file
                    File.WriteAllBytes(filepath, data);
                }
            };

            // Set the callback to the ImageReader
            reader.SetOnImageAvailableListener(readerListener, null);

            // Capture the image
            _captureSession.Capture(captureBuilder.Build(), new CameraCaptureListener(), null);
        }




    }
}
