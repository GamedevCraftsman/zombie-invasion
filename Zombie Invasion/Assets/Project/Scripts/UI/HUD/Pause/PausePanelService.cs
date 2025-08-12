using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class PausePanelService : IPausePanelService
{
    private const int Open = 1;
    private readonly GameObject _pausePanel;
    private readonly Button _countinueButton;
    private readonly Button _mainMenuButton;
    private readonly UISettings _uiSettings;
    private readonly IStopGameService _stopGameService;

    public PausePanelService(GameObject pausePanel, Button countinueButton, Button mainMenuButton, UISettings uiSettings)
    {
        _pausePanel = pausePanel;
        _countinueButton = countinueButton;
        _mainMenuButton = mainMenuButton;
        _uiSettings = uiSettings;

        _stopGameService = new StandartStopGameService();
    }
    
    public void Show()
    {
        _stopGameService.Stop();
        ResetButtons();
            
        Sequence showPanel = DOTween.Sequence();
        _pausePanel.SetActive(true);
        
        showPanel.SetUpdate(true)
            .AppendCallback(() => EnableButtons(false))
            .Append(_countinueButton.transform.DOScale(Open, _uiSettings.ShowContinueButtonTime).SetEase(_uiSettings.ShowPauseButtonsEase))
            .Append(_mainMenuButton.transform.DOScale(Open, _uiSettings.ShowMainMenuButtonTime).SetEase(_uiSettings.ShowPauseButtonsEase))
            .OnComplete(() => EnableButtons(true));
    }

    public void Hide()
    {
        _pausePanel.SetActive(false);
        _stopGameService.Continue();
    }

    private void EnableButtons(bool isOn)
    {
        _countinueButton.enabled = isOn;
        _mainMenuButton.enabled = isOn;
    }

    private void ResetButtons()
    {
        _countinueButton.transform.localScale = Vector3.zero;
        _mainMenuButton.transform.localScale = Vector3.zero;
    }
}