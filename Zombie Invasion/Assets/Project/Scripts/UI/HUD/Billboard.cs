using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Повертаємо об'єкт у бік камери
            transform.forward = mainCamera.transform.forward;
        }
    }
}