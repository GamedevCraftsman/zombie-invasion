using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : BaseManager, IUIManager
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button continueButton;

    protected override Task Initialize()
    {
        SubscribeOnEvents();
        SetButtonsEvents();
        return Task.CompletedTask;
    }

    private void SetButtonsEvents()
    {
        restartButton.onClick.AddListener(() => ShowGameOverUI(false));
        continueButton.onClick.AddListener(() => ShowGameOverUI(true));
    }

    private void SubscribeOnEvents()
    {
    }

    private void UnsubscribeFromEvents()
    {
    }

    public void ShowGameUI()
    {
        throw new System.NotImplementedException();
    }

    public void ShowGameOverUI(bool victory)
    {
        if (victory)
        {
            EventBus.Fire(new CarReachedEndEvent());
        }
        else
        {
            EventBus.Fire(new RestarGameEvent());
        }
    }

    public void HideAllUI()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateHealthBar(float normalizedHealth)
    {
        throw new System.NotImplementedException();
    }

    private void OnWinGame(CarReachedEndEvent carReachedEndEvent)
    {
    }

    private void OnLoseGame(GameOverEvent gameOverEvent)
    {
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}