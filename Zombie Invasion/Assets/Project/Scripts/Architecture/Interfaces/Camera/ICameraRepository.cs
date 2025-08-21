using System.Collections.Generic;

public interface ICameraRepository
{
    void InitializeCameras(CameraConfig[] configs);
    bool TryGetCameraByType(CameraType type, out CameraConfig config);
    bool TryGetCameraByName(string name, out CameraConfig config);
    IEnumerable<CameraConfig> GetAllCameras();
}