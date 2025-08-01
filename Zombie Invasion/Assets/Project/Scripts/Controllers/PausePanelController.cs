using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PausePanelController : BaseController
{
    [Header("Pause panel")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    
    private IPausePanelService _pausePanelService;
    private UISettings _uiSettings;
    
    [Inject]
    public void Construct(UISettings uiSettings)
    {
        _uiSettings = uiSettings;
    }
    private void Init()
    {
        _pausePanelService = new PausePanelService(pausePanel, continueButton, mainMenuButton, _uiSettings);
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
            Debug.LogError(e);
        }
        
        return Task.CompletedTask;
    }

    #region Events
    
    private void Subscribe()
    {
        EventBus.Subscribe<PauseEvent>(OnPause);
        EventBus.Subscribe<UnpauseEvent>(OnUnpause);
        EventBus.Subscribe<ToMainMenuEvent>(OnMainMenu);
    }

    private void Unsubscribe()
    {
        EventBus?.Unsubscribe<PauseEvent>(OnPause);
        EventBus?.Unsubscribe<UnpauseEvent>(OnUnpause);
        EventBus?.Unsubscribe<ToMainMenuEvent>(OnMainMenu);
    }

    private void OnPause(PauseEvent pauseEvent)
    {
        _pausePanelService.Show();
    }

    private void OnUnpause(UnpauseEvent unpauseEvent)
    {
        _pausePanelService.Hide();
    }

    private void OnMainMenu(ToMainMenuEvent toMainMenuEvent)
    {
        _pausePanelService.Hide();
        
        EventBus.Fire(new GameOverEvent(false));
        EventBus.Fire(new RestarGameEvent());
    }
    
    #endregion

    private void OnDestroy()
    {
        Unsubscribe();
    }
}