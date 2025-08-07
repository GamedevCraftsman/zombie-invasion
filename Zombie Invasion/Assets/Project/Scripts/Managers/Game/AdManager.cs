using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AdManager : BaseManager
{
    private IAdService _adService;
    private ITransitionService _transitionService;

    [Inject]
    public void Construct(IAdService adService, ITransitionService transitionService)
    {
        _adService = adService;
        _transitionService = transitionService;
    }

    protected override Task Initialize()
    {
        try
        {
            SubscribeEvents();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return Task.CompletedTask;
    }

    private void SubscribeEvents()
    {
        EventBus.Subscribe<ShowReliveAdEvent>(OnShowReliveAd);
    }

    private void UnsubscribeEvents()
    {
        EventBus?.Unsubscribe<ShowReliveAdEvent>(OnShowReliveAd);
    }

    private void OnShowReliveAd(ShowReliveAdEvent showReliveAdEvent)
    {
        Debug.LogWarning("ShowReliveAdEvent");
        _adService.ShowRewardedAd(GiveReward);
    }

    private void GiveReward()
    {
        //Here you can add Reward (function, event, coins or smth. else)
        _transitionService.ChangeEvent(PlayEvents);
    }

    private void PlayEvents()
    {
        EventBus.Fire(new RestarGameEvent());
        EventBus.Fire(new EndReliveAdEvent());
    }
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    } 
}