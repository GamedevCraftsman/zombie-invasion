using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnMapManager : BaseManager
{
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private GameObject groundTilesContainer;
    
    private List<GameObject> groundTiles = new List<GameObject>();
    private Vector3 startPosition;

    // Public access to ground tiles for enemy spawn system
    public List<GameObject> GroundTiles => groundTiles;

    protected override Task Initialize()
    {
        SetStartPosition();
        ManageGroundTiles(gameSettings.MapLength);
        return Task.CompletedTask;
    }

    private void SetStartPosition()
    {
        startPosition = new Vector3(0, 0, gameSettings.DistanceBetweenTiles);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Spawning map");
            ManageGroundTiles(gameSettings.MapLength);
        }
    }

    //Перевіряє кількість тайлів землі та розсташовує їх по порядку з заданим кроком
    public void ManageGroundTiles(int requiredCount)
    {
        // Очищуємо список від null об'єктів (якщо такі є)
        groundTiles.RemoveAll(tile => tile == null);

        // Спочатку спавнимо недостатні тайли (якщо потрібно)
        SpawnMissingTiles(requiredCount);

        // Потім переносимо всі тайли на нові позиції
        RepositionAllTiles();
    }

    //Спавнить недостатні тайли (якщо їх не вистачає)
    private void SpawnMissingTiles(int requiredCount)
    {
        int currentCount = groundTiles.Count;

        if (currentCount < requiredCount)
        {
            int tilesToSpawn = requiredCount - currentCount;

            for (int i = 0; i < tilesToSpawn; i++)
            {
                GameObject newTile = Instantiate(gameSettings.MapTilePrefab, groundTilesContainer.transform);
                groundTiles.Add(newTile);
            }
        }
    }

    //Переносить всі існуючі тайли на нові позиції від останнього тайла
    private void RepositionAllTiles()
    {
        // Знаходимо стартову позицію для переносу тайлів
        Vector3 repositionStartPosition = GetRepositionStartPositionAdvanced();

        // Переносимо всі тайли на нові позиції
        for (int i = 0; i < groundTiles.Count; i++)
        {
            if (groundTiles[i] != null)
            {
                Vector3 newTilePosition =
                    repositionStartPosition + Vector3.forward * (i * gameSettings.DistanceBetweenTiles);
                groundTiles[i].transform.position = newTilePosition;
            }
        }
    }
    
    //Альтернативний метод для більш складного перенесення
    private Vector3 GetRepositionStartPositionAdvanced()
    {
        // Знаходимо найдальший тайл по Z координаті
        float maxZ = float.MinValue;
        bool foundAnyTile = false;

        foreach (GameObject tile in groundTiles)
        {
            if (tile != null)
            {
                foundAnyTile = true;
                if (tile.transform.position.z > maxZ)
                {
                    maxZ = tile.transform.position.z + gameSettings.DistanceBetweenTiles;
                }
            }
        }

        if (foundAnyTile)
        {
            // Повертаємо позицію після найдальшого тайла
            return new Vector3(startPosition.x, startPosition.y, maxZ);
        }
        else
        {
            // Якщо тайлів немає, використовуємо стартову позицію
            return startPosition;
        }
    }
}