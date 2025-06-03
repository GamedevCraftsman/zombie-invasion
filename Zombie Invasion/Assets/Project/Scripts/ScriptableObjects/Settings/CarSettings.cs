using UnityEngine;

[CreateAssetMenu(fileName = "CarSettings", menuName = "Game/CarSettings")]
public class CarSettings : ScriptableObject
{
    [Header("Movement")] 
    [SerializeField] private Vector3 carStartPosition;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 8f;

    [Header("Health")] 
    [SerializeField] private int maxHP = 100;

    #region Public Values

    public float Speed => speed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
    public int MaxHP => maxHP;
    public Vector3 CarStartPosition => carStartPosition;

    #endregion
}