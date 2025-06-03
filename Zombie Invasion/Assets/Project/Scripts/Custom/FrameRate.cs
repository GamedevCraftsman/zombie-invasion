using UnityEngine;

public class FrameRate : MonoBehaviour
{
    [SerializeField] private int maxFrameRate = 60;
    private void Awake()
    {
        Application.targetFrameRate = maxFrameRate;
    }
}
