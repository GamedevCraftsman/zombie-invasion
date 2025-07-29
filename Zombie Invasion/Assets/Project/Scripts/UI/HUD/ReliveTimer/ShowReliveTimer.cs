using DG.Tweening;
using UnityEngine;

public class ShowReliveTimer : ITimer
{
    private readonly GameObject _backDarkPanel;
    private readonly CanvasGroup _timer;

    public bool IsOpened { get; private set; }

    public ShowReliveTimer(GameObject backDarkPanel, CanvasGroup timer)
    {
        _backDarkPanel = backDarkPanel;
        _timer = timer;
    }

    public void ShowTimerPanel()
    {
        ManageBackDarkPanel(true);
        ShowTimer();
    }

    public void HideTimerPanel()
    {
        ManageBackDarkPanel(false);
        HideTimer();
    }
    
    private void ManageBackDarkPanel(bool isOn)
    {
        _backDarkPanel.SetActive(isOn);
    }

    private void ShowTimer()
    {
        Sequence showTimer = DOTween.Sequence();

        showTimer.Append(_timer.DOFade(1f, 1f))
            .OnComplete(() =>
            {
                IsOpened = true;
                _timer.interactable = true;
                Debug.LogWarning("Is opened: " + IsOpened);
            });
    }

    private void HideTimer()
    {
        _timer.alpha = 0;
        _timer.interactable = false;
        
        IsOpened = false;
    }
}
