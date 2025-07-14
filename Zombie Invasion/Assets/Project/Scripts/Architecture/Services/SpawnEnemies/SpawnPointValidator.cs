using System.Collections.Generic;
using UnityEngine;

public class SpawnPointValidator : ISpawnPointValidator
{
    public bool IsValid(Vector3 point, IEnumerable<Vector3> existingPoints, float minDistance)
    {
        foreach (var existing in existingPoints)
        {
            if (Vector3.Distance(point, existing) < minDistance)
                return false;
        }
        return true;
    }
}