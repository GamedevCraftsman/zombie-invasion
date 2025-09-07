using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Objects")]
    [SerializeField] GameObject mapTilePrefab;
    [SerializeField] GameObject checkpointTilePrefab;
    [Header("Properties")]
    [SerializeField, Min(2)] int mapLength = 60;
    [SerializeField] float distanceBetweenTiles = 0.5f;
    
    #region Public values

    public int MapLength => mapLength;

    public GameObject MapTilePrefab => mapTilePrefab;

    public float DistanceBetweenTiles => distanceBetweenTiles;
    public GameObject CheckpointTilePrefab => checkpointTilePrefab;
    
    #endregion
}