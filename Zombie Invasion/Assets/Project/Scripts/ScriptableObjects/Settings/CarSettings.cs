using UnityEngine;

[CreateAssetMenu(fileName = "CarSettings", menuName = "Game/Car Settings")]
public class CarSettings : ScriptableObject
{
    [Header("Movement")] 
    [SerializeField] private Vector3 carStartPosition;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float acceleration = 5f;

    [Header("Health")] 
    [SerializeField] private int maxHp = 100;

    #region Public Values

    public float Speed => speed;
    public float Acceleration => acceleration;
    public int MaxHp => maxHp;
    public Vector3 CarStartPosition => carStartPosition;

    #endregion
}