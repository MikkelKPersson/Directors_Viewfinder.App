using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.OS;
using Android.Util;
using Android.Views;
using Java.Lang;

namespace Directors_Viewfinder.Platforms.Android.Camera
{
    public class CameraStateManager
    {
        private CameraDevice _cameraDevice;
        private CameraCaptureSession _captureSession;
        private CaptureRequest.Builder _previewRequestBuilder;
        private readonly object _lock = new();

        private List<string> _cameraIds;
        private int _currentCameraIndex;


        private CameraManager _cameraManager;
        private Handler _handler;
        private SurfaceTexture _surfaceTexture;
        private CameraUtilities _cameraUtilities;

        private static CameraStateManager _instance;

        private CameraStateManager() { } // Make the constructor private

        // Add a static factory method
        public static CameraStateManager Create(CameraManager cameraManager, Handler handler, SurfaceTexture surfaceTexture)
        {
            var manager = new CameraStateManager();
            manager.Initialize(cameraManager, handler, surfaceTexture);
            return manager;
        }

        public void SetSurfaceTexture(SurfaceTexture surfaceTexture)
        {
            _surfaceTexture = surfaceTexture;
        }

        public CameraDevice CameraDevice
        {
            get
            {
                lock (_lock)
                {
                    return _cameraDevice;
                }
            }
            set
            {
                lock (_lock)
                {
                    _cameraDevice = value;
                }
            }
        }

        public CameraCaptureSession CaptureSession
        {
            get
            {
                lock (_lock)
                {
                    return _captureSession;
                }
            }
            set
            {
                lock (_lock)
                {
                    _captureSession = value;
                }
            }
        }

        public CaptureRequest.Builder PreviewRequestBuilder
        {
            get
            {
                lock (_lock)
                {
                    return _previewRequestBuilder;
                }
            }
            set
            {
                lock (_lock)
                {
                    _previewRequestBuilder = value;
                }
            }
        }



        public static CameraStateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CameraStateManager();
                }
                return _instance;
            }
        }
        public void Initialize(CameraManager cameraManager, Handler handler, SurfaceTexture surfaceTexture)
        {
            _cameraManager = cameraManager;
            _handler = handler;
            _surfaceTexture = surfaceTexture;

            // Initialize CameraUtilities
            _cameraUtilities = new CameraUtilities(cameraManager);
            _cameraUtilities.PrintCameraInfo();
            _currentCameraIndex = 0;
        }



        public static void OpenCamera(CameraManager cameraManager, string cameraId, CameraDevice.StateCallback stateCallback, Handler handler)
        {
            Console.WriteLine("Opening camera: " + cameraId);

            cameraManager.OpenCamera(cameraId, stateCallback, handler);
        }

        public void CloseCamera()
        {
            _captureSession?.Close();
            _captureSession = null;

            _cameraDevice?.Close();
            _cameraDevice = null;
        }

        public void SwitchCamera()
        {
            lock (_lock)
            {
                if (_cameraIds == null || _cameraIds.Count == 0)
                {
                    Log.Error("CameraStateManager", "Error: Camera IDs not initialized");
                    return;
                }

                if (_surfaceTexture == null)
                {
                    Log.Error("CameraStateManager", "Error: SurfaceTexture is null");
                    return;
                }

                CloseCamera();
                int previousCameraIndex = _currentCameraIndex;
                _currentCameraIndex = (_currentCameraIndex + 1) % _cameraIds.Count;
                string newCameraId = _cameraIds[_currentCameraIndex];

                if (!string.IsNullOrEmpty(newCameraId))
                {
                    try
                    {
                        var characteristics = _cameraManager.GetCameraCharacteristics(newCameraId);
                        if (characteristics != null)
                        {
                            OpenCamera(_cameraManager, newCameraId, new CameraStateCallback(this, _surfaceTexture), _handler);
                            Log.Info("CameraStateManager", $"Switched to camera: {_currentCameraIndex} with ID: {newCameraId}");
                        }
                        else
                        {
                            Log.Error("CameraStateManager", $"Camera characteristics for ID {newCameraId} not found");
                            _currentCameraIndex = previousCameraIndex;
                        }
                    }
                    catch (CameraAccessException ex)
                    {
                        Log.Error("CameraStateManager", $"Failed to switch to camera: {newCameraId}, Exception: {ex.Message}");
                        _currentCameraIndex = previousCameraIndex;
                        // Re-initialize the previous camera
                        OpenCamera(_cameraManager, _cameraIds[_currentCameraIndex], new CameraStateCallback(this, _surfaceTexture), _handler);
                    }
                }
                else
                {
                    Log.Error("CameraStateManager", $"Invalid camera ID: {newCameraId}");
                    _currentCameraIndex = previousCameraIndex;
                    // Re-initialize the previous camera
                    OpenCamera(_cameraManager, _cameraIds[_currentCameraIndex], new CameraStateCallback(this, _surfaceTexture), _handler);
                }
            }
        }












        public static void CreateCaptureSession(CameraDevice cameraDevice,
                                                Surface target,
                                                CameraCaptureSession.StateCallback stateCallback,
                                                Handler handler)
        {
            OutputConfiguration outputConfiguration = new(target);
            IList<OutputConfiguration> configurations = new List<OutputConfiguration>() { outputConfiguration };
            cameraDevice.CreateCaptureSessionByOutputConfigurations(configurations,
                                                                    stateCallback,
                                                                    handler);
        }

        public void InitializeCamera(CameraDevice camera, SurfaceTexture surfaceTexture)
        {
            if (surfaceTexture == null)
            {
                // SurfaceTexture is not ready, return and try again later
                Console.WriteLine("Error: SurfaceTexture is null");
                return;
            }

            lock (_lock)
            {
                _cameraDevice = camera;

                // Create a CaptureRequest.Builder for the preview
                _previewRequestBuilder = camera.CreateCaptureRequest(CameraTemplate.Preview);

                // Set the TextureView's Surface as the target of the CaptureRequest.Builder
                Surface target = new(surfaceTexture);
                _previewRequestBuilder.AddTarget(target);

                // Create a CameraCaptureSession for the preview
                CreateCaptureSession(camera, target, new CaptureStateCallback(this), null);
            }
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
            Console.WriteLine("OnOpened called");
            _cameraStateManager.InitializeCamera(camera, _surfaceTexture);
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
