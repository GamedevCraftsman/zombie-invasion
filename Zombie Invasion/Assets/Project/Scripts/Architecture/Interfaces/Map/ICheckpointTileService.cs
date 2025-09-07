using UnityEngine;

public interface ICheckpointTileService
{
    GameObject CheckPointTile { get; }
    void SpawnTile(Transform container);
    void MoveTile(Vector3 position);
}