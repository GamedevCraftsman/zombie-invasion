using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Sequence = DG.Tweening.Sequence;

public class EndGameUIController : BaseController
{
    [Header("LosePanel")] 
    [SerializeField] private CanvasGroup losePanel;
    [SerializeField] private CanvasGroup restartGameButton;
    [SerializeField] private CanvasGroup restartGameLabel;

    [Header("Win Panel")] 
    [SerializeField] private CanvasGroup winPanel;
    [SerializeField] private CanvasGroup continueGameButton;
    [SerializeField] private CanvasGroup continueGameLabel;

    private const float Open = 1;
    private const float Close = 0;

    private UISettings _uiSettings;

    [Inject]
    public void Construct(UISettings uiSettings)
    {
        _uiSettings = uiSettings;
    }

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

    #region Events

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<RestarGameEvent>(RestartGame);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<CarReachedEndEvent>(OnReachedEnd);
        EventBus.Subscribe<ContinueGameEvent>(ContinueGame);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<RestarGameEvent>(RestartGame);
        EventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus?.Unsubscribe<CarReachedEndEvent>(OnReachedEnd);
        EventBus?.Unsubscribe<ContinueGameEvent>(ContinueGame);
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        if (!gameOverEvent.IsShowAd) return;
        
        OpenEndGamePanel(restartGameLabel, restartGameButton, losePanel);
    }

    private void OnReachedEnd(CarReachedEndEvent carReachedEndEvent)
    {
        OpenEndGamePanel(continueGameLabel, continueGameButton, winPanel);
    }

    private void RestartGame(RestarGameEvent restartGameEvent)
    {
        CloseEndGamePanel(losePanel, restartGameButton);
    }

    private void ContinueGame(ContinueGameEvent continueGameEvent)
    {
        CloseEndGamePanel(winPanel, continueGameButton);
    }

    #endregion

    private void CloseEndGamePanel(CanvasGroup panel, CanvasGroup button)
    {
        Sequence closePanel = DOTween.Sequence();
        panel.interactable = false;

        closePanel.Append(panel.DOFade(Close, _uiSettings.DisappearPanelTime))
            .OnComplete( () => ResetButtonPosition(button));
    }

    private void OpenEndGamePanel(CanvasGroup label, CanvasGroup button, CanvasGroup panel)
    {
        Sequence openPanel = DOTween.Sequence();
        panel.alpha = Open;
        panel.interactable = true;

        button.alpha = Close;

        openPanel.AppendCallback(() => button.interactable = false)
            .Join(label.DOFade(Open, _uiSettings.AppearPanelTime))
            .Append(button.transform.DOMoveY(_uiSettings.ButtonEndPos, _uiSettings.ButtonMoveTime))
            .SetEase(_uiSettings.ButtonMoveEase)
            .Join(button.DOFade(Open, _uiSettings.AppearPanelTime))
            .AppendCallback(() => button.interactable = true)
            .Complete();
        
    }

    private void ResetButtonPosition(CanvasGroup button)
    {
        button.transform.position = new Vector3(button.transform.position.x, _uiSettings.ButtonStartPos,
            button.transform.position.z);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}