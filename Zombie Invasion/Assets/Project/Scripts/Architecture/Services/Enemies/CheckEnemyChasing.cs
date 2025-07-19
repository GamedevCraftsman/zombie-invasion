using UnityEngine;

public class CheckEnemyChasing : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("Chase: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            enemyController.StartChasing();
        }
    }
}