using UnityEngine;

public class FrameRate : MonoBehaviour
{
    [SerializeField] private int maxFrameRate = 90;
    private void Start()
    {
        Application.targetFrameRate = maxFrameRate;
    }
}
