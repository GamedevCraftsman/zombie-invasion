using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Gameplay")]
    public float gameDuration = 60f;
    public int enemiesTarget = 10;
    
    [Header("Car Settings")]
    public float carSpeed = 5f;
    public float carMaxHealth = 100f;
    
    [Header("Camera")]
    public float cameraFollowSpeed = 2f;
    public Vector3 cameraOffset = new Vector3(0, 5, -10);
}
