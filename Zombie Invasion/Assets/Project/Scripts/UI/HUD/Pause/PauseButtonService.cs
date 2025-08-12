using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonService
{
    private const int Open = 1;
    private readonly Button _pauseButton;
    private readonly UISettings _uiSettings;
    
    public PauseButtonService(Button pauseButton, UISettings uiSettings)
    {
        _pauseButton = pauseButton;
        _uiSettings = uiSettings;
    }
    
    public void ShowPauseButton()
    {
        Sequence showPauseButton = DOTween.Sequence();
        ResetPauseButton();
        
        showPauseButton.Append(_pauseButton.transform.DOScale(Open, _uiSettings.ShowPauseButtonTime).SetEase(_uiSettings.ShowPauseButtonEase))
            .OnComplete(() => _pauseButton.enabled = true);
        
    }

    private void ResetPauseButton()
    {
        _pauseButton.transform.localScale = Vector3.zero;
        _pauseButton.enabled = false;
        _pauseButton.gameObject.SetActive(true);
    }

    public void HidePauseButton()
    {
        _pauseButton.gameObject.SetActive(false);
    }
}