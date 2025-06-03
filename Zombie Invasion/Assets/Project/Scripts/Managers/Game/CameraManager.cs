using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using Zenject;

public class CameraManager : BaseManager, ICameraManager
{
    [Inject] private IGameManager _gameManager;

    [SerializeField] private CameraConfig[] cameraConfigs;
    [SerializeField] private int basePriority = 10;
    [SerializeField] private int activePriority = 20;

    private ICameraRepository _cameraRepository;
    private ICameraPriorityManager _priorityManager;
    private CameraSwitchingService _switchingService;

    protected override async Task Initialize()
    {
        try
        {
            SubscribeToEvents();
            InitializeServices();
            SetInitialCamera();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        await Task.CompletedTask;
    }

    //Events management
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ReadyGameEvent>(OnReady);
        EventBus.Subscribe<GameOverEvent>(OnGameEnd);
        EventBus.Subscribe<CarReachedEndEvent>(OnReachedGameEnd);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<ReadyGameEvent>(OnReady);
        EventBus?.Unsubscribe<GameOverEvent>(OnGameEnd);
        EventBus?.Unsubscribe<CarReachedEndEvent>(OnReachedGameEnd);
    }

    //Initialize
    private void InitializeServices()
    {
        _cameraRepository = new CameraRepository();
        _cameraRepository.InitializeCameras(cameraConfigs);

        _priorityManager = new CameraPriorityService(_cameraRepository, basePriority);
        _switchingService = new CameraSwitchingService(_cameraRepository, _priorityManager, activePriority);
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

    //For events
    private void OnReady(ReadyGameEvent readyGameEvent)
    {
        if (CanSwitchToCamera(CameraType.Dynamic))
        {
            StartCoroutine(WaitSwitchToCamera(CameraType.Dynamic));
        }
    }

    private IEnumerator WaitSwitchToCamera(CameraType cameraType)
    {
        SwitchToCamera(cameraType);
        yield return new WaitForSeconds(0.5f);
        EventBus.Fire(new StartGameEvent());
    }

    private void OnGameEnd(GameOverEvent gameOverEvent)
    {
        if (CanSwitchToCamera(CameraType.Start))
        {
            SwitchToCamera(CameraType.Start);
        }
    }

    private void OnReachedGameEnd(CarReachedEndEvent recordedEndEvent)
    {
        if (CanSwitchToCamera(CameraType.Start))
        {
            SwitchToCamera(CameraType.Start);
        }
    }

    // ICameraStateProvider Implementation
    public CinemachineVirtualCamera GetCurrentCamera()
    {
        if (_cameraRepository.TryGetCameraByType(GetCurrentCameraType(), out var config))
        {
            return config.Camera;
        }

        return null;
    }

    public CameraType GetCurrentCameraType()
    {
        return _switchingService.CurrentCameraType;
    }

    public CameraConfig GetCameraConfig(CameraType cameraType)
    {
        _cameraRepository.TryGetCameraByType(cameraType, out var config);
        return config;
    }

    public CameraConfig GetCameraConfig(string cameraName)
    {
        _cameraRepository.TryGetCameraByName(cameraName, out var config);
        return config;
    }

    // ICameraSwitcher Implementation
    public bool CanSwitchToCamera(CameraType targetType) => _switchingService.CanSwitchToCamera(targetType);
    public bool CanSwitchToCamera(string cameraName) => _switchingService.CanSwitchToCamera(cameraName);
    public void SwitchToCamera(CameraType targetType) => _switchingService.SwitchToCamera(targetType);
    public void SwitchToCamera(string cameraName) => _switchingService.SwitchToCamera(cameraName);

    // ICameraPriorityManager Implementation
    public void SetCameraPriority(CameraType cameraType, int priority) =>
        _priorityManager.SetCameraPriority(cameraType, priority);

    public void ResetAllCameraPriorities() => _priorityManager.ResetAllCameraPriorities();

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}