using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private EnemyController controller;

    public void OnDeath()
    {
        if (controller != null)
        {
            controller.OnDeath();
        }
    }
}