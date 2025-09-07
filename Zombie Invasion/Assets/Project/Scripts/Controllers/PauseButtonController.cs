using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseButtonController : BaseController
{
    [Header("Pause button")]
    [SerializeField] private CanvasGroup pauseButtonGroup;
    [SerializeField] private Button pauseButton;
    
    private PauseButtonService _pauseButtonService;
    private UISettings _uiSettings;
    
    [Inject]
    public void Construct(UISettings uiSettings)
    {
        _uiSettings = uiSettings;
    }
    
    private void Init()
    {
        _pauseButtonService = new PauseButtonService(pauseButton, _uiSettings);
    }

    protected override Task Initialize()
    {
        try
        {
            Subscribe();
            Init();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
        
        return Task.CompletedTask;
    }

    #region Events

    private void Subscribe()
    {
        EventBus.Subscribe<StartGameEvent>(OnStartGame);
        EventBus.Subscribe<HidePauseButtonEvent>(OnHidePauseButton);
    }

    private void Unsubscribe()
    {
        EventBus?.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus?.Unsubscribe<HidePauseButtonEvent>(OnHidePauseButton);
    }

    private void OnStartGame(StartGameEvent startGameEvent)
    {
        _pauseButtonService.ShowPauseButton();
    }

    private void OnHidePauseButton(HidePauseButtonEvent hidePauseButtonEvent)
    {
        _pauseButtonService.HidePauseButton();
    }
    #endregion

    private void OnDestroy()
    {
        Unsubscribe();
    }
}