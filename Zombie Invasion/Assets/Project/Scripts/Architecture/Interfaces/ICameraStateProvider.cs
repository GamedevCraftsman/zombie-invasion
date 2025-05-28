using Cinemachine;

public interface ICameraStateProvider
{
    CinemachineVirtualCamera GetCurrentCamera();
    CameraType GetCurrentCameraType();
    CameraConfig GetCameraConfig(CameraType cameraType);
    CameraConfig GetCameraConfig(string cameraName);
}