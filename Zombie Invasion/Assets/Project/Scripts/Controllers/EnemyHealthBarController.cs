using UnityEngine;

public class EnemyHealthBarController : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] private bool lookAtCamera = true;

    private Camera mainCamera;
    private Canvas canvas;

    private void Start()
    {
        // FindCamer
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        // Get Canvas
        canvas = GetComponent<Canvas>();

        // Set up Canvas
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = mainCamera;
        }
    }

    private void LateUpdate()
    {
        if (!gameObject.activeInHierarchy) return;

        // Positioning over enemy
        if (transform.parent != null)
        {
            transform.position = transform.parent.position + offset;
        }

        // Rotate To Camera
        if (lookAtCamera && mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
    }
}