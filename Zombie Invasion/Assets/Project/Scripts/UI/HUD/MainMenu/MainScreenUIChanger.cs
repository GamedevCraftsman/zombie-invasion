using DG.Tweening;
using UnityEngine;

public class MainScreenUIChanger : IMainScreenUIChanger
{
    private const int Open = 1;
    private const int Close = 0;
    
    private readonly CanvasGroup _mainMenuCanvasGroup;

    public MainScreenUIChanger(CanvasGroup mainMenuCanvasGroup)
    {
        _mainMenuCanvasGroup = mainMenuCanvasGroup;
    }
    
    public void CloseMainMenu(UISettings uiSettings)
    {
        Sequence closeMainMenu = DOTween.Sequence();
        
        closeMainMenu.Append(_mainMenuCanvasGroup.DOFade(Close, uiSettings.ShowHideProgressUITime))
            .AppendCallback(() => _mainMenuCanvasGroup.gameObject.SetActive(false));
    }

    public void OpenMainMenu(UISettings uiSettings)
    {
        Sequence openMainMenu = DOTween.Sequence();

        openMainMenu.AppendCallback(() => _mainMenuCanvasGroup.gameObject.SetActive(true))
            .Append(_mainMenuCanvasGroup.DOFade(Open, uiSettings.ShowHideProgressUITime));
    }
}