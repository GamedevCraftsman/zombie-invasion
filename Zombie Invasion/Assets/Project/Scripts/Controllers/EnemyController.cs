using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public enum EnemieAnimations
{
    Run,
    Idle,
    Death
}

public class EnemyController : BaseController
{
    [Header("Components")] [SerializeField]
    private Rigidbody rb;

    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Canvas healthBarCanvas;
    [SerializeField] private UnityEngine.UI.Image healthBarFill;

    [Header("Debug")] [SerializeField, ReadOnly]
    private bool isChasing;

    [SerializeField, ReadOnly] private bool isDead;
    [SerializeField, ReadOnly] private bool hasAttacked;
    [SerializeField, ReadOnly] private int currentHealth;

    private Transform _playerTransform;
    private float _distanceToPlayer;
    private EnemieAnimations _enemieAnimation;

    [Inject] private EnemySettings _data;
    public event Action<EnemyController> OnEnemyDied;

    protected override Task Initialize()
    {
        try
        {
            Initialized();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return Task.CompletedTask;
    }

    private void Initialized()
    {
        // Assign player
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _playerTransform = player.transform;

        // Ensure Rigidbody
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.freezeRotation = true;

        // Initialize health
        currentHealth = _data.maxHealth;

        // Hide health bar initially
        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        PlayIdleAnimation();
        Debug.Log("Enemy initialized");
    }

    private void Update()
    {
        if (isDead || _playerTransform == null) return;

        CalculateDistanceToPlayer();
    }

    private void CalculateDistanceToPlayer()
    {
        _distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
        if (_distanceToPlayer <= _data.aggroRadius)
        {
            if (!isChasing)
                StartChasing();
            ChasePlayer();
        }
        else if (isChasing)
        {
            StopChasing();
        }
    }

    private void StartChasing()
    {
        isChasing = true;
        PlayRunAnimation();
    }

    private async void StopChasing()
    {
        isChasing = false;
        await PlayDeathAnimationAndDie();
    }

    private void ChasePlayer()
    {
        if (rb == null) return;

        Vector3 direction = (_playerTransform.position - transform.position).normalized;
        direction.y = 0;

        Vector3 movement = direction * _data.moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot,
                _data.rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead || hasAttacked) return;
        if (other.CompareTag("Player"))
            AttackPlayer();
    }

    private async void AttackPlayer()
    {
        hasAttacked = true;

        if (EventBus != null)
            EventBus.Fire(new PlayerDamagedEvent(_data.damage));

        await PlayDeathAnimationAndDie();
    }

    public async void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);

        ShowHealthBar();
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            await PlayDeathAnimationAndDie();
        }
    }

    private async Task PlayDeathAnimationAndDie()
    {
        if (isDead) return;

        PlayDeathAnimation();

        await WaitForDeathAnimationAsync();

        Die();
    }

    private async Task WaitForDeathAnimationAsync()
    {
        string deathAnimationName = EnemieAnimations.Death.ToString();

        await Task.Yield();

        while (true)
        {
            if (enemyAnimator == null) break;

            var stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(deathAnimationName) && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }

            await Task.Yield();
        }
    }

    private void ShowHealthBar()
    {
        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(true);
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / _data.maxHealth;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        isChasing = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        OnEnemyDied?.Invoke(this);
    }

    public void ResetForPooling()
    {
        isChasing = false;
        isDead = false;
        hasAttacked = false;
        currentHealth = _data.maxHealth;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = false;
        }

        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        PlayIdleAnimation();
    }

    // Animation methods
    private void PlayIdleAnimation() => enemyAnimator.SetTrigger(EnemieAnimations.Idle.ToString());
    private void PlayRunAnimation() => enemyAnimator.SetTrigger(EnemieAnimations.Run.ToString());
    private void PlayDeathAnimation() => enemyAnimator.SetTrigger(EnemieAnimations.Death.ToString());

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _data ? _data.aggroRadius : 0f);
        if (_playerTransform != null)
        {
            Gizmos.color = isChasing ? Color.green : Color.gray;
            Gizmos.DrawLine(transform.position, _playerTransform.position);
        }
    }
}

// ReadOnly attribute for Inspector display
public class ReadOnlyAttribute : PropertyAttribute
{
}