using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UIController : BaseController
{
    [Header("LosePanel")]
    [SerializeField] private CanvasGroup losePanel;
    [SerializeField] private CanvasGroup restartGameButton;
    [SerializeField] private CanvasGroup restartGamePanel;

    protected override Task Initialize()
    {
        try
        {
            SubscribeToEvents();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
        return Task.CompletedTask;
    }

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<RestarGameEvent>(RestartGame);
        EventBus.Subscribe<GameOverEvent>(OpenRestartGamePanel);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<RestarGameEvent>(RestartGame);
        EventBus?.Unsubscribe<GameOverEvent>(OpenRestartGamePanel);
    }
    
    private void OpenRestartGamePanel(GameOverEvent gameOverEvent)
    {
        Sequence openPanel = DOTween.Sequence();
        losePanel.alpha = 1;
        losePanel.interactable = true;
        
        openPanel.AppendCallback(() => restartGameButton.interactable = false)
            .Join(restartGamePanel.DOFade(1,2f))
            .Append(restartGameButton.transform.DOMoveY(190,3f)).SetEase(Ease.OutBack)
            .Join(restartGameButton.DOFade(1,2f))
            .AppendCallback(() => restartGameButton.interactable = true);
    }

    private void RestartGame(RestarGameEvent restartGameEvent)
    {
        losePanel.interactable = false;
        losePanel.DOFade(0, 1f);
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
