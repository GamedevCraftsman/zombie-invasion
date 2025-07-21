using UnityEngine;
using Zenject;

public class EnemyHealthBarController : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private Vector3 offset = new(0, 2f, 0);
    [SerializeField] private bool lookAtCamera = true;
    [SerializeField] private Canvas canvas;

    private Camera _mainCamera;

    [Inject]
    public void Construct(Camera mainCamera)
    {
        _mainCamera = mainCamera;
        
        Init();
    } 

    private void Init()
    {
        SetUpCanvas();
    }

    private void SetUpCanvas()
    {
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = _mainCamera;
        }
    }
    
    private void LateUpdate()
    {
        if (!gameObject.activeInHierarchy) return;
    
        SetOverEnemy();
        RotateToCamera();
    }

    private void SetOverEnemy()
    {
        if (transform.parent != null)
        {
            transform.position = transform.parent.position + offset;
        }
    }

    private void RotateToCamera()
    {
        if (lookAtCamera && _mainCamera != null)
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up);
        }
    }
}