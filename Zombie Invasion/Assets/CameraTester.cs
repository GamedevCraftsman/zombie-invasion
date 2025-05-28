using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CameraTester : BaseController
{
    [Inject] private CameraController cameraController;
    
    [SerializeField] private KeyCode toggleKey = KeyCode.Q;
    [SerializeField] private KeyCode startKey = KeyCode.E;
    [SerializeField] private KeyCode dynamicKey = KeyCode.R;
    
    private CameraType[] availableCameras = {
        CameraType.Start,
        CameraType.Dynamic
    };
    
    private int currentIndex = 0;
    
    protected override async Task Initialize()
    {
        EventBus.Subscribe<CameraSwitchedEvent>(OnCameraSwitched);
        Debug.Log("CameraTester initialized");
        Debug.Log($"Press {toggleKey} to toggle between cameras");
        Debug.Log($"Press {startKey} for Start camera");
        Debug.Log($"Press {dynamicKey} for Dynamic camera");
        
        await Task.CompletedTask;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleCameras();
        }
        else if (Input.GetKeyDown(startKey))
        {
            Debug.Log("Starting camera");
            cameraController.SwitchToCamera(CameraType.Start);
        }
        else if (Input.GetKeyDown(dynamicKey))
        {
            Debug.Log("Dynamic camera");
            cameraController.SwitchToCamera(CameraType.Dynamic);
        }
    }
    
    private void ToggleCameras()
    {
        var currentCamera = cameraController.GetCurrentCameraType();
        var targetCamera = currentCamera == CameraType.Start ? CameraType.Dynamic : CameraType.Start;
        
        Debug.Log($"Toggling from {currentCamera} to {targetCamera}");
        cameraController.SwitchToCamera(targetCamera);
    }
    
    private void OnCameraSwitched(CameraSwitchedEvent cameraEvent)
    {
        Debug.Log($"[CameraTester] Camera switched from {cameraEvent.FromCamera} to {cameraEvent.ToCamera}");
        Debug.Log($"[CameraTester] Active camera: {cameraEvent.CameraName}");
    }
    
    private void OnDestroy()
    {
        EventBus?.Unsubscribe<CameraSwitchedEvent>(OnCameraSwitched);
    }
    
    /*private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Camera Test Controls:", EditorStyles.boldLabel);
        
        if (GUILayout.Button($"{toggleKey} - Toggle Cameras"))
            ToggleCameras();
            
        if (GUILayout.Button($"{startKey} - Start Camera"))
            cameraController.SwitchToCamera(CameraType.Start);
            
        if (GUILayout.Button($"{dynamicKey} - Dynamic Camera"))
            cameraController.SwitchToCamera(CameraType.Dynamic);
        
        GUILayout.Space(10);
        GUILayout.Label($"Current Camera: {cameraController.GetCurrentCameraType()}");
        
        GUILayout.EndArea();
    }*/
}
