using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class CameraManager : BaseManager
{
    [SerializeField] private CameraConfig[] cameraConfigs;
    [SerializeField] private int basePriority = 10;
    [SerializeField] private int activePriority = 20;
    [SerializeField] private float waitToStartLvlTime = 0.5f;

    private ICameraRepository _cameraRepository;
    private ICameraPriorityManager _priorityManager;
    private CameraSwitchingService _switchingService;
    private WaitForSeconds _waitToReady;

    //Camera States
    private const CameraType MenuCameraType = CameraType.Start;
    private const CameraType PlayCameraType = CameraType.Dynamic;

    protected override Task Initialize()
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

        return Task.CompletedTask;
    }

    #region Initialization

    private void InitializeServices()
    {
        _cameraRepository = new CameraRepository();
        _cameraRepository.InitializeCameras(cameraConfigs);

        _priorityManager = new CameraPriorityService(_cameraRepository, basePriority);
        _switchingService = new CameraSwitchingService(_cameraRepository, _priorityManager, activePriority);
        _waitToReady = new WaitForSeconds(waitToStartLvlTime);
    }

    private void SetInitialCamera()
    {
        SwitchToCamera(MenuCameraType);
    }

    #endregion

    #region Events

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ReadyGameEvent>(OnReady);
        EventBus.Subscribe<RestarGameEvent>(OnGameRestart);
        EventBus.Subscribe<CarReachedEndEvent>(OnReachedGameEnd);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<ReadyGameEvent>(OnReady);
        EventBus?.Unsubscribe<RestarGameEvent>(OnGameRestart);
        EventBus?.Unsubscribe<CarReachedEndEvent>(OnReachedGameEnd);
    }

    private void OnReady(ReadyGameEvent readyGameEvent)
    {
        if (CanSwitchToCamera(PlayCameraType))
        {
            StartCoroutine(WaitSwitchToCamera(PlayCameraType));
        }
    }

    private void OnGameRestart(RestarGameEvent restartGameEvent)
    {
        SwitchToCamera(MenuCameraType);
    }

    private void OnReachedGameEnd(CarReachedEndEvent recordedEndEvent)
    {
        SwitchToCamera(MenuCameraType);
    }

    #endregion

    private IEnumerator WaitSwitchToCamera(CameraType cameraType)
    {
        SwitchToCamera(cameraType);
        yield return _waitToReady;
        EventBus.Fire(new StartGameEvent());
    }

    #region CameraSwitcher Implementation

    public bool CanSwitchToCamera(CameraType targetType) => _switchingService.CanSwitchToCamera(targetType);
    public bool CanSwitchToCamera(string cameraName) => _switchingService.CanSwitchToCamera(cameraName);
    public void SwitchToCamera(CameraType targetType) => _switchingService.SwitchToCamera(targetType);
    public void SwitchToCamera(string cameraName) => _switchingService.SwitchToCamera(cameraName);

    #endregion

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}