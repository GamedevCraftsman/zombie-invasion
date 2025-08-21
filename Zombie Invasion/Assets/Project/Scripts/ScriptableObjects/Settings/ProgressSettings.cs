using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ProgressSettings", menuName = "Game/Progress Settings")]
public class ProgressSettings : ScriptableObject
{
    [Header("General")] 
    [SerializeField] private int actionInterval = 5;
    
    [Header("Map")] 
    [SerializeField] private int mapLenghtIncrease = 1;

    [FormerlySerializedAs("enemyIncrease")]
    [Header("Enemies")] 
    [SerializeField] private int enemiesIncrease = 2;

    #region Public Values

    public int MapLenghtIncrease => mapLenghtIncrease;
    public int EnemiesIncrease => enemiesIncrease;
    public int ActionInterval => actionInterval;
    #endregion
}