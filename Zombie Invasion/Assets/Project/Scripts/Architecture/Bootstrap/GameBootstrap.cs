using System;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private BaseManager[] managersInOrder;
    [SerializeField] private BaseController[] controllersInOrder;
    
    private async void Awake()
    {
        try
        {
            foreach (var manager in managersInOrder)
            {
                await manager.InitializeAsync();
            }
        
            OnAllManagersInitialized();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void Start()
    {
        try
        {
            foreach (var controller in controllersInOrder)
            {
                await controller.InitializeAsync();
            }

            OnAllControllersInitialized();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void OnAllControllersInitialized()
    {
        Debug.Log("All controllers initialized");
    }

    void OnAllManagersInitialized()
    {
        Debug.Log("All managers initialized");
    }
}
