public interface ICameraSwitcher
{
    bool CanSwitchToCamera(CameraType targetType);
    bool CanSwitchToCamera(string cameraName);
    void SwitchToCamera(CameraType targetType);
    void SwitchToCamera(string cameraName);
}