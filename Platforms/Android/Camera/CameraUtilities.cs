using Android.Hardware.Camera2;
using Android.OS;
using Android.Util;
using SizeF = Android.Util.SizeF;

namespace Directors_Viewfinder.Platforms.Android.Camera
{

    public class CameraUtilities
    {
        private CameraManager _cameraManager;

        public CameraUtilities(CameraManager cameraManager)
        {
            _cameraManager = cameraManager;
        }

        public List<string> GetBackFacingCameraIds()
        {
            var backFacingCameraIds = new List<string>();
            var cameraIds = _cameraManager.GetCameraIdList();

            Log.Info("CameraUtilities", $"Available cameras: {string.Join(", ", cameraIds)}");

            foreach (var cameraId in cameraIds)
            {
                var characteristics = _cameraManager.GetCameraCharacteristics(cameraId);
                var facing = (LensFacing)((Java.Lang.Integer)characteristics.Get(CameraCharacteristics.LensFacing)).IntValue();

                if (facing == LensFacing.Back)
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                    {
                        var capabilities = characteristics.Get(CameraCharacteristics.RequestAvailableCapabilities);
                        var capabilitiesList = capabilities.ToArray<RequestAvailableCapabilities>();

                        if (capabilitiesList.Contains(RequestAvailableCapabilities.LogicalMultiCamera))
                        {
                            var physicalCameraIds = characteristics.PhysicalCameraIds;

                            foreach (var physicalCameraId in physicalCameraIds)
                            {
                                if (!backFacingCameraIds.Contains(physicalCameraId))
                                {
                                    backFacingCameraIds.Add(physicalCameraId);
                                }
                            }
                        }
                        else
                        {
                            backFacingCameraIds.Add(cameraId);
                        }
                    }
                    else
                    {
                        backFacingCameraIds.Add(cameraId);
                    }
                }

                Log.Info("CameraUtilities", $"Selected camera: {cameraId}");
            }
            Log.Info("CameraUtilities", $"Available cameras: {string.Join(", ", backFacingCameraIds)}");
            return backFacingCameraIds;
        }
        public float CalculateFullFrameEquivalentFocalLength(string cameraId, float actualFocalLength)
        {
            var characteristics = _cameraManager.GetCameraCharacteristics(cameraId);
            var sensorSize = (SizeF)characteristics.Get(CameraCharacteristics.SensorInfoPhysicalSize);

            // Calculate the diagonal of the sensor size
            var sensorDiagonal = Math.Sqrt(Math.Pow(sensorSize.Width, 2) + Math.Pow(sensorSize.Height, 2));

            // Calculate the diagonal of the full frame sensor size (36mm x 24mm)
            var fullFrameDiagonal = Math.Sqrt(Math.Pow(36, 2) + Math.Pow(24, 2));

            // Calculate the crop factor
            var cropFactor = fullFrameDiagonal / sensorDiagonal;

            // Calculate the full frame equivalent focal length
            var fullFrameEquivalentFocalLength = actualFocalLength * cropFactor;

            return (float)fullFrameEquivalentFocalLength;
        }

        public void PrintCameraInfo()
        {
            var cameraIds = GetBackFacingCameraIds();

            foreach (var cameraId in cameraIds)
            {
                var characteristics = _cameraManager.GetCameraCharacteristics(cameraId);
                var sensorSize = (SizeF)characteristics.Get(CameraCharacteristics.SensorInfoPhysicalSize);
                var focalLengths = (float[])characteristics.Get(CameraCharacteristics.LensInfoAvailableFocalLengths);

                foreach (var focalLength in focalLengths)
                {
                    var fullFrameEquivalentFocalLength = CalculateFullFrameEquivalentFocalLength(cameraId, focalLength);

                    Log.Info("CameraUtilities", 
                        $"Camera ID: {cameraId}," +
                        $"Sensor Size: {sensorSize.Width}mm x {sensorSize.Height}mm, " +
                        $"Focal Length: {focalLength}mm, " +
                        $"Full Frame Equivalent Focal Length: {fullFrameEquivalentFocalLength}mm");
                }
            }
        }

    }
}
