using System.Threading.Tasks;
using Zenject;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    [Inject] protected IEventBus EventBus;
    
    public async Task InitializeAsync()
    {
        await Initialize();
    }
    
    protected abstract Task Initialize();
}
