using System.Threading.Tasks;
using Zenject;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    [Inject] protected IEventBus EventBus { get; }

    public async Task InitializeAsync()
    {
        await Initialize();
    }
    
    protected abstract Task Initialize();
}
