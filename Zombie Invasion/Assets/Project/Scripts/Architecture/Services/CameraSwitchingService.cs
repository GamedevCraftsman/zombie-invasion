using UnityEngine;

public class CameraSwitchingService : ICameraSwitcher
{
    private readonly ICameraRepository cameraRepository;
    private readonly ICameraPriorityManager priorityManager;
    private readonly int activePriority;
    
    private CameraType currentCameraType = CameraType.Start;
    
    public CameraType CurrentCameraType => currentCameraType;
    
    public CameraSwitchingService(ICameraRepository repository, ICameraPriorityManager priorityManager, int activePriority)
    {
        this.cameraRepository = repository;
        this.priorityManager = priorityManager;
        this.activePriority = activePriority;
    }
    
    public bool CanSwitchToCamera(CameraType targetType)
    {
        return currentCameraType != targetType && cameraRepository.TryGetCameraByType(targetType, out _);
    }
    
    public bool CanSwitchToCamera(string cameraName)
    {
        return cameraRepository.TryGetCameraByName(cameraName, out _);
    }
    
    public void SwitchToCamera(CameraType targetType)
    {
        if (!CanSwitchToCamera(targetType))
        {
            Debug.LogWarning($"Cannot switch to camera {targetType}");
            return;
        }
        
        if (cameraRepository.TryGetCameraByType(targetType, out var targetConfig))
        {
            priorityManager.ResetAllCameraPriorities();
            targetConfig.Camera.Priority = activePriority;
            currentCameraType = targetType;
        }
    }
    
    public void SwitchToCamera(string cameraName)
    {
        if (!CanSwitchToCamera(cameraName))
        {
            Debug.LogWarning($"Cannot switch to camera '{cameraName}'");
            return;
        }
        
        if (cameraRepository.TryGetCameraByName(cameraName, out var config))
        {
            SwitchToCamera(config.Type);
        }
    }
}