using UnityEngine;
using Zenject;

public class EnemyDeathEffectPoolHandler : IPoolable<EnemyDeathEffectsService>
{
    private readonly EnemySpawnSettings _settings;
    private readonly DiContainer _container;

    public EnemyDeathEffectPoolHandler(EnemySpawnSettings settings, DiContainer container)
    {
        _settings = settings;
        _container = container;
    }

    public EnemyDeathEffectsService OnCreate()
    {
        var effectObject = _container.InstantiatePrefab(_settings.DeathEffectPrefab);
        var effectService = effectObject.GetComponent<EnemyDeathEffectsService>();

        if (effectService == null)
        {
            Debug.LogErrorFormat("DeathEffect prefab must have EnemyDeathEffectsService");
            return null;
        }
       
        return effectService;
    }

    public void OnGet(EnemyDeathEffectsService item)
    {
        if (item == null) return;
        
        item.gameObject.SetActive(true);
        
        item.PlayParticles(); 
    }

    public void OnRelease(EnemyDeathEffectsService item)
    {
        if(item == null) return;
        
        //Reset effect state
        item.StopParticles();
        item.gameObject.SetActive(false);
    }

    public void OnDestroy(EnemyDeathEffectsService item)
    {
        if (item != null && item.gameObject != null)
        {
            Object.Destroy(item.gameObject);
        }
    }
}