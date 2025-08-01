using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : BaseManager, IUIManager
{
    [Header("General")] [SerializeField] private Button respawnButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button continueButton;
    [Header("Pause")] [SerializeField] private Button pauseButton;
    [SerializeField] private Button unpauseButton;
    [SerializeField] private Button mainMenuButton;

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
        respawnButton.onClick.AddListener(ShowRewardedAd);
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
        }
        else
        {
            Debug.LogWarning("Restart");
            EventBus.Fire(new RestarGameEvent());
        }
    }

    #endregion

    #region Show Rewarded Ad

    private void ShowRewardedAd()
    {
        respawnButton.interactable = false;

        EventBus.Fire(new ShowReliveAdEvent());
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
        EventBus.Fire(new ToMainMenuEvent());
    }

    #endregion
}