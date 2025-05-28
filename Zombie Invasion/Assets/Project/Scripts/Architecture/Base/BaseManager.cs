using System.Threading.Tasks;
using Zenject;
using UnityEngine;

public abstract class BaseManager : MonoBehaviour
{
    [Inject] protected IEventBus EventBus;
    
    public async Task InitializeAsync()
    {
        await Initialize();
    }
    
    protected abstract Task Initialize();
}
