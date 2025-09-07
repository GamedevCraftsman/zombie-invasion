using System.Collections;
using UnityEngine;

public class EnemyDeathEffectsService : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;

    private IPool<EnemyDeathEffectsService> _pool;
    private Coroutine _stopParticlesCoroutine;
    
    public void Init(IPool<EnemyDeathEffectsService> pool)
    {
        _pool = pool;
    }
    
    public void PlayParticles()
    {
        particles.Play();

        _stopParticlesCoroutine = StartCoroutine(ReleaseAfterStop());
    }

    public void StopParticles()
    {
        if (_stopParticlesCoroutine != null)
            StopCoroutine(_stopParticlesCoroutine);
        
        particles.Stop();
    }

    private IEnumerator ReleaseAfterStop()
    {
        yield return new WaitUntil(() => !particles.isPlaying);
        
        _pool.Release(this);
    }
}