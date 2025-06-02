using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class UIController : BaseController
{
    [Header("LosePanel")]
    [SerializeField] private CanvasGroup losePanel;
    [SerializeField] private CanvasGroup restartGameButton;
    [SerializeField] private CanvasGroup restartGameLabel;
    
    [Header("Win Panel")]
    [SerializeField] private CanvasGroup winPanel;
    [SerializeField] private CanvasGroup continueGameButton;
    [SerializeField] private CanvasGroup continueGameLabel;

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
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<CarReachedEndEvent>(OnReachedEnd);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<RestarGameEvent>(RestartGame);
        EventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus?.Unsubscribe<CarReachedEndEvent>(OnReachedEnd);
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        OpenEndGamePanel(restartGameLabel, restartGameButton, losePanel);
    }

    private void OnReachedEnd(CarReachedEndEvent carReachedEndEvent)
    {
        OpenEndGamePanel(continueGameLabel, continueGameButton, winPanel);
    }
    
    private void OpenEndGamePanel(CanvasGroup label, CanvasGroup button, CanvasGroup panel)
    {
        Sequence openPanel = DOTween.Sequence();
        panel.alpha = 1;
        panel.interactable = true;
        
        openPanel.AppendCallback(() => button.interactable = false)
            .Join(label.DOFade(1,2f))
            .Append(button.transform.DOMoveY(190,3f)).SetEase(Ease.OutBack)
            .Join(button.DOFade(1,2f))
            .AppendCallback(() => button.interactable = true);
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
