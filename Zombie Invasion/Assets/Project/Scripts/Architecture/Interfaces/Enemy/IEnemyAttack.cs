using System;
using UnityEngine;

public interface IEnemyAttack
{
    void AttackPlayer(Action attackAction, Canvas targetCanvas);
}