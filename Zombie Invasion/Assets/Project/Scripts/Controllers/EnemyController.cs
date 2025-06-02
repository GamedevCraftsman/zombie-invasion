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
    [SerializeField, ReadOnly] private bool canMove;
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
        if (isDead || _playerTransform == null || !canMove) return;

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

    private void StopChasing()
    {
        isChasing = false;
        PlayDeathAnimationAndDie();
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

    private void AttackPlayer()
    {
        hasAttacked = true;
        canMove = false;
        
        if (EventBus != null)
            EventBus.Fire(new PlayerDamagedEvent(_data.damage));

        StopChasing();
    }

    // Don`t delete
    /*public async void TakeDamage(int damageAmount)
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
    }*/

    private void PlayDeathAnimationAndDie()
    {
        PlayDeathAnimation();
    }

    public void OnDeath()
    {
        Die();
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

        Debug.LogWarning("Die");
        OnEnemyDied?.Invoke(this);
    }

    public void ResetForPooling()
    {
        canMove = true;
        isChasing = false;
        isDead = false;
        hasAttacked = false;
        currentHealth = _data.maxHealth;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
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