using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Objects")]
    [SerializeField] GameObject mapTilePrefab;
    [Header("Properties")]
    [SerializeField] int mapLength = 60;
    [SerializeField] float distanceBetweenTiles = 0.5f;

    public int MapLength => mapLength;
    public GameObject MapTilePrefab => mapTilePrefab;
    public float DistanceBetweenTiles => distanceBetweenTiles;
}