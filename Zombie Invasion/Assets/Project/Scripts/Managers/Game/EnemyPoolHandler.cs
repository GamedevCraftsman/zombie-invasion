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
        
        _container.Inject(enemyController);
        
        return enemyController;
    }
    
    public async void OnGet(EnemyController item)
    {
        if (item == null) return;
        
        item.gameObject.SetActive(true);

        try
        {
            await item.InitializeAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize enemy: {e.Message}");
            item.gameObject.SetActive(false);
        }
    }
    
    public void OnRelease(EnemyController item)
    {
        if (item == null) return;
        
        // Reset enemy state
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