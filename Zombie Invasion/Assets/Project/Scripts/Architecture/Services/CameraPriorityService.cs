using UnityEngine;

public class CameraPriorityService : ICameraPriorityManager
{
    private readonly ICameraRepository cameraRepository;
    private readonly int basePriority;
    
    public CameraPriorityService(ICameraRepository repository, int basePriority)
    {
        this.cameraRepository = repository;
        this.basePriority = basePriority;
    }
    
    public void SetCameraPriority(CameraType cameraType, int priority)
    {
        if (cameraRepository.TryGetCameraByType(cameraType, out var config))
        {
            config.Camera.Priority = priority;
        }
        else
        {
            Debug.LogWarning($"Camera {cameraType} not found for priority setting");
        }
    }
    
    public void ResetAllCameraPriorities()
    {
        foreach (var config in cameraRepository.GetAllCameras())
        {
            if (config.Camera != null)
            {
                config.Camera.Priority = basePriority;
            }
        }
    }
}