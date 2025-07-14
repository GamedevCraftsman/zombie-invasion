using UnityEngine;

public class CameraSwitchingService : ICameraSwitcher
{
    private readonly ICameraRepository _cameraRepository;
    private readonly ICameraPriorityManager _priorityManager;
    private readonly int _activePriority;

    private CameraType _currentCameraType = CameraType.None;

    public CameraSwitchingService(ICameraRepository repository, ICameraPriorityManager priorityManager,
        int activePriority)
    {
        _cameraRepository = repository;
        _priorityManager = priorityManager;
        _activePriority = activePriority;
    }

    #region Switch ny type

    public bool CanSwitchToCamera(CameraType targetType)
    {
        return _currentCameraType != targetType && _cameraRepository.TryGetCameraByType(targetType, out _);
    }

    public void SwitchToCamera(CameraType targetType)
    {
        if (!CanSwitchToCamera(targetType))
        {
            Debug.LogWarning($"Don`t switch to camera {targetType}\n(By type)");
            return;
        }

        if (_cameraRepository.TryGetCameraByType(targetType, out var targetConfig))
        {
            _priorityManager.SetCameraPriority(targetConfig.Type, _activePriority);
            _currentCameraType = targetType;
        }
    }

    #endregion

    #region Switch by name

    public bool CanSwitchToCamera(string cameraName)
    {
        return _cameraRepository.TryGetCameraByName(cameraName, out _);
    }

    public void SwitchToCamera(string cameraName)
    {
        if (!CanSwitchToCamera(cameraName))
        {
            Debug.LogWarning($"Don`t switch to camera '{cameraName}'\n(By name)");
            return;
        }

        if (_cameraRepository.TryGetCameraByName(cameraName, out var config))
        {
            SwitchToCamera(config.Type);
        }
    }

    #endregion
}