using UnityEngine;

public class CheckEnemyAttack : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    
    private void OnTriggerEnter(Collider other)
    {
        if (enemyController.CanAttack()) return;

        if (other.CompareTag("Player"))
        {
            enemyController.AttackPlayer();
        }
    }
}