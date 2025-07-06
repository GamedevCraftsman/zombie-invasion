using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Objects")]
    [SerializeField] GameObject mapTilePrefab;
    [Header("Properties")]
    [SerializeField, Min(2)] int mapLength = 60;
    [SerializeField] float distanceBetweenTiles = 0.5f;

    public int MapLength => mapLength;
    public GameObject MapTilePrefab => mapTilePrefab;
    public float DistanceBetweenTiles => distanceBetweenTiles;
    
    public float LvlLenghtCalculation(Transform carTransform)
    {
        //Round to the nearest tenth.
        float lvlLenght = Mathf.Round((carTransform.position.z 
                                       + (mapLength - 1) 
                                       * distanceBetweenTiles) * 10f) / 10f; 
        
        return lvlLenght;
    }
}