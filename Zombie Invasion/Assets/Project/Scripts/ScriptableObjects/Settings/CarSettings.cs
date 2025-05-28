using UnityEngine;

[CreateAssetMenu(fileName = "CarSettings", menuName = "Game/CarSettings")]
public class CarSettings : ScriptableObject
{
    [SerializeField] float carSpeed = 5f;
    [SerializeField] float carMaxHealth = 100f;
    
    public float CarSpeed => carSpeed;
    public float CarMaxHealth => carMaxHealth;
}