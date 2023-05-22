using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.OS;
using Android.Views;
using Java.Lang;
using static Android.Hardware.Camera2.CameraCaptureSession;

namespace Directors_Viewfinder.Android.Camera
{
    public class CameraStateManager
    {
        private CameraDevice _cameraDevice;
        private CameraCaptureSession _captureSession;
        private CaptureRequest.Builder _previewRequestBuilder;

        public CameraDevice CameraDevice
        {
            get { return _cameraDevice; }
            set { _cameraDevice = value; }
        }

        public CameraCaptureSession CaptureSession
        {
            get { return _captureSession; }
            set { _captureSession = value; }
        }

        public CaptureRequest.Builder PreviewRequestBuilder
        {
            get { return _previewRequestBuilder; }
            set { _previewRequestBuilder = value; }
        }

        public static void OpenCamera(CameraManager cameraManager, string cameraId, CameraDevice.StateCallback stateCallback, Handler handler)
        {
            cameraManager.OpenCamera(cameraId, stateCallback, handler);
        }

        public void CloseCamera()
        {
            _captureSession?.Close();
            _captureSession = null;

            _cameraDevice?.Close();
            _cameraDevice = null;
        }

        public static void CreateCaptureSession(CameraDevice cameraDevice, Surface surface, CameraCaptureSession.StateCallback stateCallback, Handler handler)
        {
            OutputConfiguration outputConfiguration = new(surface);
            IList<OutputConfiguration> configurations = new List<OutputConfiguration>() { outputConfiguration };
            cameraDevice.CreateCaptureSessionByOutputConfigurations(configurations,
                                                                    stateCallback,
                                                                    handler);
        }



        public void DisposeCaptureSession()
        {
            _captureSession?.Dispose();
            _captureSession = null;
        }
    }

    public class CaptureStateCallback : CameraCaptureSession.StateCallback
    {
        private readonly CameraStateManager _cameraStateManager;

        public CaptureStateCallback(CameraStateManager cameraStateManager)
        {
            _cameraStateManager = cameraStateManager;
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            // The camera is already closed
            if (_cameraStateManager.CameraDevice == null)
            {
                return;
            }

            // When the session is ready, we start displaying the preview
            _cameraStateManager.CaptureSession = session;
            try
            {
                // Auto focus should be continuous for camera preview
                _cameraStateManager.PreviewRequestBuilder.Set(CaptureRequest.ControlAfMode,
                    (int)ControlAFMode.ContinuousPicture);

                // Finally, we start displaying the camera preview
                CaptureRequest previewRequest = _cameraStateManager.PreviewRequestBuilder.Build();
                _cameraStateManager.CaptureSession.SetRepeatingRequest(previewRequest, new CaptureCallback(), null);


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

    public class CameraStateCallback : CameraDevice.StateCallback
    {
        private readonly CameraStateManager _cameraStateManager;
        private readonly SurfaceTexture _surfaceTexture;

        public CameraStateCallback(CameraStateManager cameraStateManager, SurfaceTexture surfaceTexture)
        {
            _cameraStateManager = cameraStateManager;
            _surfaceTexture = surfaceTexture;
        }

        public override void OnOpened(CameraDevice camera)
        {
            // Camera opened successfully. Now we can start the preview
            _cameraStateManager.CameraDevice = camera;

            // Create a CaptureRequest.Builder for the preview
            _cameraStateManager.PreviewRequestBuilder = camera.CreateCaptureRequest(CameraTemplate.Preview);

            // Set the TextureView's Surface as the target of the CaptureRequest.Builder
            Surface surface = new(_surfaceTexture);
            _cameraStateManager.PreviewRequestBuilder.AddTarget(surface);

            // Create a CameraCaptureSession for the preview
            CameraStateManager.CreateCaptureSession(camera, surface, new CaptureStateCallback(_cameraStateManager), null);
        }

        public override void OnDisconnected(CameraDevice camera)
        {
            // Camera has been disconnected
            _cameraStateManager.CloseCamera();
        }

        public override void OnError(CameraDevice camera, CameraError error)
        {
            // An error occurred while opening the camera
            _cameraStateManager.CloseCamera();
        }
    }

    public class CaptureCallback : CameraCaptureSession.CaptureCallback
    {
        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            // Do nothing
        }
    }

    public class HandlerExecutor : Java.Lang.Object, Java.Util.Concurrent.IExecutor
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

}
