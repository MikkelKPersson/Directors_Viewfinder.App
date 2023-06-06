using Android.Hardware.Camera2;
using Android.OS;
using Android.Util;

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
    }
}
