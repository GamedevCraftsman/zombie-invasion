using System.Threading.Tasks;
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
        if (item == null) return;
        
        item.gameObject.SetActive(true);
        
        // Переконуємося, що ініціалізація завершена перед поверненням
        try
        {
            await item.InitializeAsync();
            Debug.Log($"Enemy initialized at position: {item.transform.position}");
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
        
        Debug.Log($"Enemy released from position: {item.transform.position}");
    }
    
    public void OnDestroy(EnemyController item)
    {
        if (item != null && item.gameObject != null)
        {
            Object.Destroy(item.gameObject);
        }
    }
}