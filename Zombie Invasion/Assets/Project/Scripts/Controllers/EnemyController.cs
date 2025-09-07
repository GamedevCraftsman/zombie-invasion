using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class EnemyController : BaseController
{
    [Header("Components")] [SerializeField]
    private Rigidbody rb;

    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Canvas healthBarCanvas;
    [SerializeField] private UnityEngine.UI.Image healthBarFill;
    [SerializeField] private SphereCollider agroCollider;
    [SerializeField] private Collider[] allColliders;

    private Transform _playerTransform;
    private EnemyAnimations _enemyAnimation;

    #region Temporary values

    private int _currentHealth;

    private WaitForFixedUpdate _waitForFixedUpdate;
    
    private Coroutine _chaseCoroutine;

    #endregion

    #region Injections

    private EnemySettings _enemySettings;

    private ICarController _carController;

    private IEnemyAttack _enemyAttack;

    private IEnemyHealthBarService _enemyHealthBarService;
    private IPool<EnemyDeathEffectsService> _deathEffectsServicePool;

    #endregion

    #region Public values

    public event Action<EnemyController> OnEnemyDied;
    public Canvas HealthBarCanvas => healthBarCanvas;
    public IEnemyAttack EnemyAttack => _enemyAttack;

    #endregion

    [Inject]
    private void Construct(EnemySettings enemySettings, ICarController carController, IEnemyAttack enemyAttack,
        IEnemyHealthBarService enemyHealthBarService, IPool<EnemyDeathEffectsService> deathEffectsServicePool)
    {
        _enemySettings = enemySettings;
        _carController = carController;
        _enemyAttack = enemyAttack;
        _enemyHealthBarService = enemyHealthBarService;
        _deathEffectsServicePool = deathEffectsServicePool;
        
        _waitForFixedUpdate = new WaitForFixedUpdate();
    }

    #region Initialization

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
        _playerTransform = _carController.Car.transform;

        // Set agro radius
        agroCollider.radius = _enemySettings.AggroRadius;
        
        // Initialize health
        _currentHealth = _enemySettings.MaxHealth;

        // Hide health bar initially
        ManageHealthBar(false, healthBarCanvas);

        PlayIdleAnimation();
    }

    #endregion

    #region Chasing

    public void StartChasing()
    {
        _chaseCoroutine = StartCoroutine(ChasePlayer());
        PlayRunAnimation();
    }

    public void StopChasing()
    {
        if (_chaseCoroutine != null)
            StopCoroutine(_chaseCoroutine);

        PlayDeathAnimationAndDie();
    }

    private IEnumerator ChasePlayer()
    {
        while (true)
        {
            if (rb == null) break;

            Vector3 direction = (_playerTransform.position - transform.position).normalized;
            direction.y = 0;

            Vector3 movement = direction * (_enemySettings.MoveSpeed * Time.deltaTime);
            rb.MovePosition(transform.position + movement);

            if (direction != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot,
                    _enemySettings.RotationSpeed * Time.deltaTime);
            }
            
            yield return _waitForFixedUpdate;
        }
    }

    private void PlayDeathAnimationAndDie()
    {
        ManageColliders(false);
        
        Die();
    }

    #endregion

    #region Animation methods

    private void PlayIdleAnimation() => enemyAnimator.SetTrigger(EnemyAnimations.Idle.ToString());

    private void PlayRunAnimation() => enemyAnimator.SetTrigger(EnemyAnimations.Run.ToString());
    
    #endregion

    #region Death methods

    private void Die()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        Debug.Log("Dead");
        PlayDeathEffects();
        OnEnemyDied?.Invoke(this);
    }

    public void ResetForPooling()
    {
        ManageColliders(true);
        _currentHealth = _enemySettings.MaxHealth;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        PlayIdleAnimation();
    }

    private void PlayDeathEffects()
    {
        var effect =_deathEffectsServicePool.Get();
        
        effect.Init(_deathEffectsServicePool);
        effect.transform.position = new Vector3(transform.position.x, EnemyModelMiddle(), transform.position.z);
        effect.PlayParticles();
    }

    private float EnemyModelMiddle()
    {
        return transform.localPosition.y + (transform.localScale.y / 2);
    }
    
    #endregion

    public void TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Max(0, _currentHealth);

        ManageHealthBar(true, healthBarCanvas);
        _enemyHealthBarService.UpdateHealthBar(healthBarFill, _currentHealth, _enemySettings.MaxHealth);

        if (_currentHealth <= 0)
        {
            ManageHealthBar(false, healthBarCanvas);
            StopChasing();
        }
    }

    private void ManageColliders(bool isOn)
    {
        foreach (var collider in allColliders)
        {
            collider.enabled = isOn;
        }
    }

    private void ManageHealthBar(bool isOn, Canvas healthBar) =>
        _enemyHealthBarService.ManageHealthBar(isOn, healthBar);
}