using UnityEngine;

public class EnemyHealthBarController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] private bool lookAtCamera = true;
    
    private Camera mainCamera;
    private Canvas canvas;
    
    private void Start()
    {
        // Знаходимо камеру
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        // Отримуємо Canvas
        canvas = GetComponent<Canvas>();
        
        // Налаштовуємо Canvas
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = mainCamera;
        }
    }
    
    private void LateUpdate()
    {
        if (!gameObject.activeInHierarchy) return;
        
        // Позиціонуємо над ворогом
        if (transform.parent != null)
        {
            transform.position = transform.parent.position + offset;
        }
        
        // Повертаємо до камери
        if (lookAtCamera && mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
    }
}