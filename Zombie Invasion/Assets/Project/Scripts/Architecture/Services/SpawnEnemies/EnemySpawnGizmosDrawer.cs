#if UNITY_EDITOR
using UnityEngine;
using Zenject;

[ExecuteAlways]
public class EnemySpawnGizmosDrawer : MonoBehaviour
{
    [Inject] private EnemySpawnController _controller;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        var points = _controller.AllSpawnPoints;
        if (points == null || points.Count == 0) return;

        Gizmos.color = Color.green;
        float size = 0.05f;
        foreach (var p in points)
            Gizmos.DrawWireSphere(p, size);
    }
}
#endif