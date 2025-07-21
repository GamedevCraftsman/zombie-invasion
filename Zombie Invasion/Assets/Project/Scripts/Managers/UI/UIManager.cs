using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : BaseManager, IUIManager
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button continueButton;

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
    }

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
}