public interface ICameraManager : ICameraStateProvider, ICameraSwitcher, ICameraPriorityManager
{
    void Initialize(CameraConfig[] configs, int basePriority = 10, int activePriority = 20);
}