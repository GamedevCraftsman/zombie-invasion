using UnityEngine;

public class CheckEnemyChasing : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyController.StartChasing();
        }
    }
}