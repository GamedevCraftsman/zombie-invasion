using System.Collections.Generic;
using UnityEngine;

public interface ISpawnPointValidator
{
    bool IsValid(Vector3 point, IEnumerable<Vector3> existingPoints, float minDistance);
}