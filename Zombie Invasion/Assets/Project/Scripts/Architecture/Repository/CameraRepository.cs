using System.Collections.Generic;
using UnityEngine;

public class CameraRepository : ICameraRepository
{
    private Dictionary<CameraType, CameraConfig> cameraMap;
    private Dictionary<string, CameraConfig> cameraNameMap;
    
    public void InitializeCameras(CameraConfig[] configs)
    {
        cameraMap = new Dictionary<CameraType, CameraConfig>();
        cameraNameMap = new Dictionary<string, CameraConfig>();
        
        foreach (var config in configs)
        {
            if (config.Camera != null)
            {
                cameraMap[config.Type] = config;
                cameraNameMap[config.DisplayName] = config;
            }
        }
    }
    
    public bool TryGetCameraByType(CameraType type, out CameraConfig config)
    {
        return cameraMap.TryGetValue(type, out config);
    }
    
    public bool TryGetCameraByName(string name, out CameraConfig config)
    {
        return cameraNameMap.TryGetValue(name, out config);
    }
    
    public IEnumerable<CameraConfig> GetAllCameras()
    {
        return cameraMap.Values;
    }
}