using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Game/CameraSettings")]
public class CameraSettings : ScriptableObject
{
    [SerializeField] float cameraFollowSpeed = 2f;
    [SerializeField] Vector3 cameraOffset = new Vector3(0, 5, -10);
    
    public float CameraFollowedSpeed => cameraFollowSpeed;
    public Vector3 CameraOffset => cameraOffset;
}