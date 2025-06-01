using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using System.Linq;

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

    [Inject] private IEventBus eventBus;
    [Inject] private EnemySettings data;
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
        currentHealth = data.maxHealth;

        // Hide health bar initially
        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        PlayIdleAnimation();
        Debug.Log("Enemy initialized");
    }

    private void Update()
    {
        if (isDead || _playerTransform == null) return;

        _distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
        if (_distanceToPlayer <= data.aggroRadius)
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
        Debug.Log("Enemy started chasing");
    }

    private async void StopChasing()
    {
        isChasing = false;
        //PlayIdleAnimation();
        await PlayDeathAnimationAndDie();
        Debug.Log("Enemy stopped chasing");
    }

    private void ChasePlayer()
    {
        if (rb == null) return;

        Vector3 direction = (_playerTransform.position - transform.position).normalized;
        direction.y = 0;

        Vector3 movement = direction * data.moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot,
                data.rotationSpeed * Time.deltaTime);
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

        if (eventBus != null)
            eventBus.Fire(new PlayerDamagedEvent(data.damage));

        Debug.Log($"Enemy attacked player for {data.damage} damage");
        
        // Граємо анімацію смерті і чекаємо її завершення
        await PlayDeathAnimationAndDie();
    }
    
    public async void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);

        ShowHealthBar();
        UpdateHealthBar();

        Debug.Log($"Enemy took {damageAmount} damage. Health: {currentHealth}/{data.maxHealth}");

        if (currentHealth <= 0)
        {
            await PlayDeathAnimationAndDie();
        }
    }

    // Новий асинхронний метод для програвання анімації смерті та виконання Die()
    private async Task PlayDeathAnimationAndDie()
    {
        if (isDead) return; // Запобігаємо повторному виклику
        
        // Граємо анімацію смерті
        PlayDeathAnimation();
        
        // Чекаємо завершення анімації
        await WaitForDeathAnimationAsync();
        
        // Виконуємо Die() після завершення анімації
        Die();
    }

    // Асинхронний метод очікування завершення анімації смерті
    private async Task WaitForDeathAnimationAsync()
    {
        string deathAnimationName = EnemieAnimations.Death.ToString();
        
        // Чекаємо один кадр для початку анімації
        await Task.Yield();
        
        // Чекаємо поки анімація повністю не завершиться
        while (true)
        {
            if (enemyAnimator == null) break;
            
            var stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
            
            // Перевіряємо чи грається анімація смерті і чи вона завершена
            if (stateInfo.IsName(deathAnimationName) && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
            
            // Якщо анімація ще не почалася або ще грається
            await Task.Yield();
        }
        
        Debug.Log("Death animation completed!");
    }
    
    private void ShowHealthBar()
    {
        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(true);
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / data.maxHealth;
    }

    private void Die()
    {
        if (isDead) return; // Запобігаємо повторному виклику
        
        isDead = true;
        isChasing = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        Debug.Log("Enemy died");

        // Викликаємо event для системи пулінгу
        OnEnemyDied?.Invoke(this);

        // Не деактивуємо об'єкт тут - це зробить пул
    }

    public void ResetForPooling()
    {
        // Скидаємо всі стани
        isChasing = false;
        isDead = false;
        hasAttacked = false;
        currentHealth = data.maxHealth;

        // Скидаємо фізику
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = false;
        }

        // Ховаємо UI
        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        // Відновлюємо анімацію
        PlayIdleAnimation();
    }

    // Animation methods
    private void PlayIdleAnimation() => Debug.Log("Idle animation ON");
    private void PlayRunAnimation() => enemyAnimator.SetTrigger(EnemieAnimations.Run.ToString());
    private void PlayDeathAnimation() => enemyAnimator.SetTrigger(EnemieAnimations.Death.ToString());

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data ? data.aggroRadius : 0f);
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