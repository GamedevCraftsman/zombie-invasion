using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIManager : BaseManager, IUIManager
{
    [Header("General")] 
    [SerializeField] private Button respawnButton;
    [SerializeField] private TMP_Text respawnButtonText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button continueButton;
    
    [Header("Pause")] 
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button unpauseButton;
    [SerializeField] private Button mainMenuButton;

    private ITransitionService _transitionService;
    [Inject]
    public void Construct(ITransitionService transitionService)
    {
        _transitionService = transitionService;
    }
    
    protected override Task Initialize()
    {
        try
        {
            SetButtonsEvents();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return Task.CompletedTask;
    }
    
    private void SetButtonsEvents()
    {
        restartButton.onClick.AddListener(() => ShowGameOverUI(false));
        continueButton.onClick.AddListener(() => ShowGameOverUI(true));

        //Pause
        pauseButton.onClick.AddListener(Pause);
        unpauseButton.onClick.AddListener(Unpause);
        mainMenuButton.onClick.AddListener(MainMenu);
    }

    #region Game Over UI

    public void ShowGameOverUI(bool victory)
    {
        if (victory)
        {
            EventBus.Fire(new ContinueGameEvent());
            EventBus.Fire(new AllowStartGameEvent());
        }
        else
        {
            Debug.LogWarning("Restart");
            _transitionService.ChangeEvent(() => EventBus.Fire(new RestarGameEvent()));
        }
    }

    #endregion

    #region Pause

    private void Pause()
    {
        EventBus.Fire(new PauseEvent());
    }

    private void Unpause()
    {
        EventBus.Fire(new UnpauseEvent());
    }

    private void MainMenu()
    {
        _transitionService.ChangeEvent(() => EventBus.Fire(new ToMainMenuEvent()));
    }

    #endregion
}