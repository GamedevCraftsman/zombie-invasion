using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyManager : BaseManager, IEnemyManager
{
    protected override Task Initialize()
    {
        throw new NotImplementedException();
    }

    public void SpawnEnemies()
    {
        throw new NotImplementedException();
    }

    public void RegisterEnemyDeath(Vector3 position)
    {
        throw new NotImplementedException();
    }

    public int EnemiesAlive { get; }
    public event Action<int> OnEnemyCountChanged;
}
