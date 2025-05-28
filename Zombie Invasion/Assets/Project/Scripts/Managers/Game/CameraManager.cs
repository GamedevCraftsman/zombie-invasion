using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;

public class CameraManager : BaseManager, ICameraManager
{
    [SerializeField] private CameraConfig[] cameraConfigs;
    [SerializeField] private int basePriority = 10;
    [SerializeField] private int activePriority = 20;
    
    private ICameraRepository cameraRepository;
    private ICameraPriorityManager priorityManager;
    private CameraSwitchingService switchingService;
    
    protected override async Task Initialize()
    {
        InitializeServices();
        SetInitialCamera();
        
        Debug.Log("CameraManager initialized");
        await Task.CompletedTask;
    }
    
    private void InitializeServices()
    {
        cameraRepository = new CameraRepository();
        cameraRepository.InitializeCameras(cameraConfigs);
        
        priorityManager = new CameraPriorityService(cameraRepository, basePriority);
        switchingService = new CameraSwitchingService(cameraRepository, priorityManager, activePriority);
    }
    
    private void SetInitialCamera()
    {
        if (CanSwitchToCamera(CameraType.Start))
        {
            SwitchToCamera(CameraType.Start);
        }
        else if (cameraConfigs.Length > 0)
        {
            SwitchToCamera(cameraConfigs[0].Type);
        }
    }
    
    public void Initialize(CameraConfig[] configs, int basePriority = 10, int activePriority = 20)
    {
        this.basePriority = basePriority;
        this.activePriority = activePriority;
        cameraConfigs = configs;
        InitializeServices();
    }
    
    // ICameraStateProvider Implementation
    public CinemachineVirtualCamera GetCurrentCamera()
    {
        if (cameraRepository.TryGetCameraByType(GetCurrentCameraType(), out var config))
        {
            return config.Camera;
        }
        return null;
    }
    
    public CameraType GetCurrentCameraType()
    {
        return switchingService.CurrentCameraType;
    }
    
    public CameraConfig GetCameraConfig(CameraType cameraType)
    {
        cameraRepository.TryGetCameraByType(cameraType, out var config);
        return config;
    }
    
    public CameraConfig GetCameraConfig(string cameraName)
    {
        cameraRepository.TryGetCameraByName(cameraName, out var config);
        return config;
    }
    
    // ICameraSwitcher Implementation
    public bool CanSwitchToCamera(CameraType targetType) => switchingService.CanSwitchToCamera(targetType);
    public bool CanSwitchToCamera(string cameraName) => switchingService.CanSwitchToCamera(cameraName);
    public void SwitchToCamera(CameraType targetType) => switchingService.SwitchToCamera(targetType);
    public void SwitchToCamera(string cameraName) => switchingService.SwitchToCamera(cameraName);
    
    // ICameraPriorityManager Implementation
    public void SetCameraPriority(CameraType cameraType, int priority) => priorityManager.SetCameraPriority(cameraType, priority);
    public void ResetAllCameraPriorities() => priorityManager.ResetAllCameraPriorities();
}