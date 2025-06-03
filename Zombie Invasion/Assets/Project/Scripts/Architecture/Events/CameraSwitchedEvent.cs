using Cinemachine;

public class CameraSwitchedEvent
{
    public CameraType FromCamera { get; }
    public CameraType ToCamera { get; }
    public CinemachineVirtualCamera Camera { get; }
    public string CameraName { get; }

    public CameraSwitchedEvent(CameraType from, CameraType to, CinemachineVirtualCamera camera, string name)
    {
        FromCamera = from;
        ToCamera = to;
        Camera = camera;
        CameraName = name;
    }
}
