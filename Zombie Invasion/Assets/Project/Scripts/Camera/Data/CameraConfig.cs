using Cinemachine;
using UnityEngine;

[System.Serializable]
public class CameraConfig
{
    [SerializeField] private CameraType cameraType;
    [SerializeField] private string displayName;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public CameraType Type => cameraType;
    public string DisplayName => displayName;
    public CinemachineVirtualCamera Camera => virtualCamera;

    public CameraConfig(CameraType type, string name, CinemachineVirtualCamera camera)
    {
        cameraType = type;
        displayName = name;
        virtualCamera = camera;
    }
}