using UnityEngine;
using Zenject;

public class FindCamera : MonoBehaviour
{
    [SerializeField] private Canvas canvas; 
    
    private Camera _mainCamera;

    [Inject]
    public void Construct(Camera mainCamera)
    {
        _mainCamera = mainCamera;
    }

    private void SetMainCamera()
    {
        canvas.worldCamera = _mainCamera;
    }
}
