using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ReliveController : BaseController
{
    [Header("General")] 
    [SerializeField] private GameObject backDarkPanel;
    [SerializeField] private CanvasGroup timer;
    [SerializeField] private Button reliveButton;
    
    [Header("Timer")] 
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image bar;

    private Coroutine _timerCoroutine;
    private ITimer _showTimer;
    private ITimeCounter _timeCounter;

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
            Init();
            SubscribeEvents();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return Task.CompletedTask;
    }

    private void Init()
    {
        _showTimer = new ShowReliveTimer(backDarkPanel, timer);
        _timeCounter = new StandartTimeCounter(_uiSettings.TimeToRelive);
    }

    #region Events

    private void SubscribeEvents()
    {
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<EndReliveAdEvent>(OnEndReliveAd);
    }

    private void UnsubscribeEvents()
    {
        EventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus?.Unsubscribe<EndReliveAdEvent>(OnEndReliveAd);
    }

    private void OnEndReliveAd(EndReliveAdEvent endReliveAdEvent)
    {
        StopCoroutine(_timerCoroutine);
        _showTimer.HideTimerPanel();
        
        reliveButton.interactable = true;
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        _timeCounter.ResetCounter(timerText, bar);
        _showTimer.ShowTimerPanel();

        _timerCoroutine = StartCoroutine(Timer());
    }

    #endregion

    private IEnumerator Timer()
    {
        yield return new WaitUntil(PanelOpened);
        Debug.LogWarning("PanelOpened: " + PanelOpened());

        WaitForSeconds wait = new WaitForSeconds(1);

        while (true)
        {
            bool isTimeGo = _timeCounter.CountDownTick(timerText, bar);

            if (!isTimeGo) break;
            yield return wait;
        }

        _showTimer.HideTimerPanel();
    }

    private bool PanelOpened()
    {
        return _showTimer.IsOpened;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}