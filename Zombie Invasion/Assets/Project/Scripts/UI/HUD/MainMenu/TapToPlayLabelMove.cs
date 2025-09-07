using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class TapToPlayLabelMove : BaseController
{
    [SerializeField] private GameObject label;

    private UISettings _uiSettings;
    
    private Vector3 _startPosition;
    private Tween _labelMoveTween;

    [Inject]
    public void Construct(UISettings uiSettings)
    {
        _uiSettings = uiSettings;
    }
    
    protected override Task Initialize()
    {
        try
        {
            StartMove();
            Subscribe();
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
        EventBus.Subscribe<StartGameEvent>(OnGameStart);
        EventBus.Subscribe<ContinueGameEvent>(OnGameContinue);
        EventBus.Subscribe<RestarGameEvent>(OnGameRestart);
    }

    private void UnSubscribe()
    {
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStart);
        EventBus?.Unsubscribe<ContinueGameEvent>(OnGameContinue);
        EventBus?.Unsubscribe<RestarGameEvent>(OnGameRestart);
    }

    private void OnGameStart(StartGameEvent gameStartEvent)
    {
        _labelMoveTween?.Kill();
        label.transform.position = _startPosition;
    }

    private void OnGameContinue(ContinueGameEvent gameContinueEvent)
    {
        StartMove();
    }

    private void OnGameRestart(RestarGameEvent gameEndEvent)
    {
        StartMove();
    }

    #endregion

    private void StartMove()
    {
        _startPosition = label.transform.position;

        _labelMoveTween = label.transform.DOMoveY(label.transform.position.y - _uiSettings.TapToPlayPositionOffset, _uiSettings.TimeToMoveTapToPlay, true)
            .SetLoops(-1, _uiSettings.TapToPlayLoop);
    }

    private void OnDestroy()
    {
        UnSubscribe();
        _labelMoveTween?.Kill();
    }
}