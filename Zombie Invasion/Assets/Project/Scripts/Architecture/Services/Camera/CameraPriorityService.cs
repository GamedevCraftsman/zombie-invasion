using UnityEngine;

public class CameraPriorityService : ICameraPriorityManager
{
    private readonly ICameraRepository _cameraRepository;
    private readonly int _basePriority;

    public CameraPriorityService(ICameraRepository repository, int basePriority)
    {
        _cameraRepository = repository;
        _basePriority = basePriority;
    }

    public void SetCameraPriority(CameraType cameraType, int priority)
    {
        if (_cameraRepository.TryGetCameraByType(cameraType, out var config))
        {
            ResetAllCameraPriorities();
            config.Camera.Priority = priority;
        }
        else
        {
            Debug.LogWarning($"Camera {cameraType} not found for priority setting");
        }
    }

    private void ResetAllCameraPriorities()
    {
        foreach (var config in _cameraRepository.GetAllCameras())
        {
            if (config.Camera != null)
            {
                config.Camera.Priority = _basePriority;
            }
        }
    }
}