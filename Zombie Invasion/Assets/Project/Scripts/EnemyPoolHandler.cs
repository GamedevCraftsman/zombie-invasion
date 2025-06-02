using UnityEngine;
using Zenject;

public class EnemyPoolHandler : IPoolable<EnemyController>
{
    private readonly EnemySpawnSettings _settings;
    private readonly DiContainer _container;
    
    public EnemyPoolHandler(EnemySpawnSettings settings, DiContainer container)
    {
        _settings = settings;
        _container = container;
    }
    
    public EnemyController OnCreate()
    {
        var enemyObject = Object.Instantiate(_settings.EnemyPrefab);
        var enemyController = enemyObject.GetComponent<EnemyController>();
        
        if (enemyController == null)
        {
            Debug.LogError("EnemyPrefab must have EnemyController component!");
            return null;
        }
        
        // Inject dependencies into your existing EnemyController
        _container.Inject(enemyController);
        
        return enemyController;
    }
    
    public async void OnGet(EnemyController item)
    {
        item.gameObject.SetActive(true);

        await item.InitializeAsync();
    }
    
    public void OnRelease(EnemyController item)
    {
        // Reset enemy state - you'll need to add this method to your EnemyController
        item.ResetForPooling();
        item.gameObject.SetActive(false);
    }
    
    public void OnDestroy(EnemyController item)
    {
        if (item != null && item.gameObject != null)
        {
            Object.Destroy(item.gameObject);
        }
    }
}