using UnityEngine;

public class FindCamera : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    private void Start()
    {
        canvas.worldCamera = Camera.main;
    }
}
