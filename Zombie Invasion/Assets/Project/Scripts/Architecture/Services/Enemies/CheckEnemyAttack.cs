using UnityEngine;
using Zenject;

public class CheckEnemyAttack : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(enemyController.EnemyAttack != null)
                enemyController.EnemyAttack.AttackPlayer(enemyController.StopChasing, enemyController.HealthBarCanvas);
            else Debug.LogWarning("EnemyAttack = null");
        }
    }
}