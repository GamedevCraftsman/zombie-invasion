using System;
using DG.Tweening;
using UnityEngine;

public class StandardTransition : ITransitionService
{
    private readonly CanvasGroup _transitionPanel;
    private readonly IEventBus _eventBus;

    public StandardTransition(CanvasGroup transitionPanel, IEventBus eventBus)
    {
        _transitionPanel = transitionPanel;
        _eventBus = eventBus;
    }

    public void ChangeEvent(Action eventAction)
    {
        Debug.LogWarning("Changing StandardTransition event");
        
        Sequence transition = DOTween.Sequence();
        _transitionPanel.alpha = 0;

        transition.SetUpdate(true)
            .AppendCallback(() => _transitionPanel.gameObject.SetActive(true))
            .Append(_transitionPanel.DOFade(1, 0.5f))
            .AppendCallback(() => eventAction?.Invoke())
            .AppendInterval(1.5f)
            .Append(_transitionPanel.DOFade(0, 0.5f))
            .AppendCallback(() => Debug.LogWarning("Finished Transition"))
            .AppendCallback(() => _transitionPanel.gameObject.SetActive(false))
            .OnComplete(() => _eventBus.Fire(new AllowStartGameEvent()));    
    }
}