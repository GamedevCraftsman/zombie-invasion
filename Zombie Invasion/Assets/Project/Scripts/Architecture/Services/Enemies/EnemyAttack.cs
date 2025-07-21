using System;
using UnityEngine;
using Zenject;

public class EnemyAttack : IEnemyAttack
{
    private readonly IEventBus _eventBus;
    private readonly IEnemyHealthBarService _enemyHealthBarService;
    private readonly EnemySettings _enemySettings;
    

    [Inject]
    public EnemyAttack(IEventBus eventBus, IEnemyHealthBarService enemyHealthBarService, EnemySettings enemySettings)
    {
        _eventBus = eventBus;
        _enemyHealthBarService = enemyHealthBarService;
        _enemySettings = enemySettings;
    }

    public void AttackPlayer(Action attackAction, Canvas targetCanvas)
    {
        attackAction?.Invoke();
        
        _enemyHealthBarService.ManageHealthBar(false, targetCanvas);
        if (_eventBus != null)
            _eventBus.Fire(new PlayerDamagedEvent(_enemySettings.Damage));
    }
}