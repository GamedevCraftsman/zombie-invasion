using System.Collections.Generic;

public class CameraRepository : ICameraRepository
{
    private Dictionary<CameraType, CameraConfig> _cameraMap;
    private Dictionary<string, CameraConfig> _cameraNameMap;
    
    public void InitializeCameras(CameraConfig[] configs)
    {
        _cameraMap = new Dictionary<CameraType, CameraConfig>();
        _cameraNameMap = new Dictionary<string, CameraConfig>();
        
        foreach (var config in configs)
        {
            if (config.Camera != null)
            {
                _cameraMap[config.Type] = config;
                _cameraNameMap[config.DisplayName] = config;
            }
        }
    }
    
    public bool TryGetCameraByType(CameraType type, out CameraConfig config)
    {
        return _cameraMap.TryGetValue(type, out config);
    }
    
    public bool TryGetCameraByName(string name, out CameraConfig config)
    {
        return _cameraNameMap.TryGetValue(name, out config);
    }
    
    public IEnumerable<CameraConfig> GetAllCameras()
    {
        return _cameraMap.Values;
    }
}