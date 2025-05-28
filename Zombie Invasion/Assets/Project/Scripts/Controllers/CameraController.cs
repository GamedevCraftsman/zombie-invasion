using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CameraController : BaseController
{
    [Inject] private ICameraManager cameraManager;
    
    protected override async Task Initialize()
    {
        Debug.Log("CameraController initialized");
        await Task.CompletedTask;
    }
    
    public void SwitchToCamera(CameraType targetType)
    {
        if (!cameraManager.CanSwitchToCamera(targetType)) return;
        
        var previousType = cameraManager.GetCurrentCameraType();
        var targetConfig = cameraManager.GetCameraConfig(targetType);
        
        cameraManager.SwitchToCamera(targetType);
        
        var switchEvent = new CameraSwitchedEvent(
            previousType,
            targetType,
            targetConfig.Camera,
            targetConfig.DisplayName
        );
        
        EventBus.Fire(switchEvent);
        Debug.Log($"Camera switched from {previousType} to {targetType}");
    }
    
    public void SwitchToCamera(string cameraName)
    {
        if (!cameraManager.CanSwitchToCamera(cameraName)) return;
        
        var cameraConfig = cameraManager.GetCameraConfig(cameraName);
        SwitchToCamera(cameraConfig.Type);
    }
    
    // Delegate calls to CameraManager
    public CameraType GetCurrentCameraType() => cameraManager.GetCurrentCameraType();
    public void SetCameraPriority(CameraType cameraType, int priority) => cameraManager.SetCameraPriority(cameraType, priority);
}