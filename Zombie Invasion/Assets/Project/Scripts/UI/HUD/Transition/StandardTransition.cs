using System;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

public class StandardTransition : ITransitionService
{
    private CanvasGroup _transitionPanel;
    private readonly IEventBus _eventBus;
    private readonly TransitionPanelSettings _transitionPanelSettings;

    private const int Open = 1;
    private const int Close = 0;
    public StandardTransition(GameObject transitionPanelPrefab, IEventBus eventBus, TransitionPanelSettings transitionPanelSettings)
    {
        MakeTransitionPanel(transitionPanelPrefab);
        _eventBus = eventBus;
        _transitionPanelSettings = transitionPanelSettings;
    }

    private void MakeTransitionPanel(GameObject transitionPanelPrefab)
    {
        var transitionPanel = Object.Instantiate(transitionPanelPrefab); 
         _transitionPanel =  transitionPanel.GetComponentInChildren<CanvasGroup>();
        Object.DontDestroyOnLoad(transitionPanel);
        
        _transitionPanel.gameObject.SetActive(false);
    }
    
    public void ChangeEvent(Action eventAction)
    {
        Sequence transition = DOTween.Sequence();
        _transitionPanel.alpha = 0;

        transition.SetUpdate(true)
            .AppendCallback(() => _transitionPanel.gameObject.SetActive(true))
            .Append(_transitionPanel.DOFade(Open, _transitionPanelSettings.FadeDuration))
            .AppendCallback(() => eventAction?.Invoke())
            .AppendInterval(_transitionPanelSettings.DelayBeforeHideTransitionPanel)
            .Append(_transitionPanel.DOFade(Close, _transitionPanelSettings.FadeDuration))
            .AppendCallback(() => _transitionPanel.gameObject.SetActive(false))
            .OnComplete(() => _eventBus.Fire(new AllowStartGameEvent()));    
    }
}