using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private BaseManager[] managersInOrder;
    [SerializeField] private BaseController[] controllersInOrder;
    
    private async void Await()
    {
        foreach (var manager in managersInOrder)
        {
            await manager.InitializeAsync();
        }
        
        OnAllManagersInitialized();
    }

    private async void Start()
    {
        foreach (var controller in controllersInOrder)
        {
            await controller.InitializeAsync();
        }

        OnAllControllersinitialized();
    }

    private void OnAllControllersinitialized()
    {
        Debug.Log("All controllers initialized");
    }

    void OnAllManagersInitialized()
    {
        Debug.Log("All managers initialized");
    }
}
