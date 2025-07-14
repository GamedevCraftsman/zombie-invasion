using System.Collections.Generic;
using UnityEngine;

public interface ISpawnPointGenerator
{
    List<Vector3> GeneratePoints(List<GameObject> tiles, EnemySpawnSettings settings);
}