using UnityEngine;
using Zenject;

public class EnemyController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemySettings data;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Canvas healthBarCanvas;
    [SerializeField] private UnityEngine.UI.Image healthBarFill;

    [Header("Debug")]
    [SerializeField, ReadOnly] private bool isChasing;
    [SerializeField, ReadOnly] private bool isDead;
    [SerializeField, ReadOnly] private bool hasAttacked;
    [SerializeField, ReadOnly] private int currentHealth;

    private Transform playerTransform;
    private float distanceToPlayer;

    [Inject] private IEventBus eventBus;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // Assign player
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

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
        if (isDead || playerTransform == null) return;

        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= data.aggroRadius)
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

    private void StopChasing()
    {
        isChasing = false;
        PlayIdleAnimation();
        Debug.Log("Enemy stopped chasing");
    }

    private void ChasePlayer()
    {
        if (rb == null) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
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

    private void AttackPlayer()
    {
        hasAttacked = true;
        PlayAttackAnimation();

        if (eventBus != null)
            eventBus.Fire(new PlayerDamagedEvent(data.damage));

        Debug.Log($"Enemy attacked player for {data.damage} damage");
        Die();
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);

        ShowHealthBar();
        UpdateHealthBar();

        Debug.Log($"Enemy took {damageAmount} damage. Health: {currentHealth}/{data.maxHealth}");

        if (currentHealth <= 0)
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
            healthBarFill.fillAmount = (float)currentHealth / data.maxHealth;
    }

    private void Die()
    {
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
        gameObject.SetActive(false);
    }

    // Animation placeholders
    private void PlayIdleAnimation()    => Debug.Log("Idle animation ON");
    private void PlayRunAnimation()     => Debug.Log("Run animation ON");
    private void PlayAttackAnimation()  => Debug.Log("Attack animation ON");

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data ? data.aggroRadius : 0f);
        if (playerTransform != null)
        {
            Gizmos.color = isChasing ? Color.green : Color.gray;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
}

// ReadOnly attribute for Inspector display
public class ReadOnlyAttribute : PropertyAttribute { }