using UnityEngine;
using Zenject;

public class Billboard : MonoBehaviour
{
    private Camera _mainCamera;

    [Inject]
    private void Construct(Camera mainCamera)
    {
        _mainCamera = mainCamera;
    }

    void LateUpdate()
    {
        if (_mainCamera != null)
        {
            transform.forward = _mainCamera.transform.forward;
        }
    }
}