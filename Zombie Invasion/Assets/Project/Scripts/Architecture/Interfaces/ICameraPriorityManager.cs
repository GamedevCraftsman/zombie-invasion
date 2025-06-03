public interface ICameraPriorityManager
{
    void SetCameraPriority(CameraType cameraType, int priority);
    void ResetAllCameraPriorities();
}