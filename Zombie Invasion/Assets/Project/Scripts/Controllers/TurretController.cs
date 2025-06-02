using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class TurretController : BaseController, ITurretController
{
    [Header("References")]
    [SerializeField] private Transform turretTransform;
    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletPool bulletPool;
    
    [Header("Settings")]
    [SerializeField] private WeaponSettings weaponSettings;
    
    // Injected dependencies
    [Inject] private IInputController inputController;
    
    // State
    private bool controlEnabled = false;
    private float currentRotationAngle = 0f;
    private bool isDragging = false;
    private Vector2 lastInputPosition;
    
    // Shooting state
    private float lastFireTime = 0f;
    
    // Properties
    public bool IsControlEnabled => controlEnabled;
    public float CurrentRotationAngle => currentRotationAngle;
    
    // Events
    public event Action<float> OnRotationChanged;
    
    protected override Task Initialize()
    {
        try
        {
            ValidateComponents();
            SubscribeToEvents();
            ResetRotation();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
        return Task.CompletedTask;
    }
    
    private void ValidateComponents()
    {
        if (turretTransform == null)
        {
            turretTransform = transform;
            Debug.LogWarning("TurretTransform is null! Use current transform");
        }
        
        if (firePoint == null)
        {
            Debug.LogError("FirePoint is null! Please assign fire point transform");
        }
        
        if (bulletPool == null)
        {
            Debug.LogError("BulletPool is null! Please assign bullet pool");
        }
        
        if (weaponSettings == null)
        {
            Debug.LogError("WeaponSettings is null!");
        }
    }
    
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<StartGameEvent>(OnGameStarted);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<CarReachedEndEvent>(OnCarReached);
    }
    
    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStarted);
        EventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus?.Unsubscribe<CarReachedEndEvent>(OnCarReached);
    }
    
    private void OnGameStarted(StartGameEvent gameStartedEvent)
    {
        EnableControl();
    }
    
    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        DisableControl();
        ResetRotation();
    }

    private void OnCarReached(CarReachedEndEvent carReachedEndEvent)
    {
        DisableControl();
        ResetRotation();
    }
    
    private void Update()
    {
        if (!controlEnabled || weaponSettings == null) return;

        HandleInput();
        HandleShooting();
    }
    
    private void HandleInput()
    {
        InputType inputType = inputController.LastInputType;
        
        if (inputType == InputType.Mouse || inputType == InputType.None)
        {
            HandleMouseInput();
        }
        else if (inputType == InputType.Touch)
        {
            HandleTouchInput();
        }
    }
    
    private void HandleShooting()
    {
        // Стрільба тільки коли гравець тримає палець/мишку (isDragging)
        if (isDragging && CanFire())
        {
            Fire();
        }
    }
    
    private bool CanFire()
    {
        return Time.time >= lastFireTime + weaponSettings.fireRate;
    }
    
    private void Fire()
    {
        if (bulletPool == null || firePoint == null) return;
        
        Bullet bullet = bulletPool.GetBullet();
        if (bullet == null) return;
        
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        
        bullet.Initialize(
            weaponSettings.bulletSpeed,
            weaponSettings.bulletDamage,
            weaponSettings.bulletLifetime,
            bulletPool
        );
        
        lastFireTime = Time.time;
    }
    
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            ContinueDragging(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }
    
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartDragging(touch.position);
                    break;
                    
                case TouchPhase.Moved when isDragging:
                    ContinueDragging(touch.position);
                    break;
                    
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    StopDragging();
                    break;
            }
        }
    }
    
    private void StartDragging(Vector2 inputPosition)
    {
        isDragging = true;
        lastInputPosition = inputPosition;
    }
    
    private void ContinueDragging(Vector2 currentInputPosition)
    {
        if (!isDragging) return;
        
        Vector2 deltaPosition = currentInputPosition - lastInputPosition;
        
        float horizontalDelta = deltaPosition.x * weaponSettings.inputSensitivity;
        
        float rotationDelta = horizontalDelta * weaponSettings.rotationSpeed * Time.deltaTime * 0.01f;
        float newAngle = currentRotationAngle + rotationDelta;
        
        newAngle = Mathf.Clamp(newAngle, -weaponSettings.maxRotationAngle, weaponSettings.maxRotationAngle);
        
        SetRotation(newAngle);
        
        lastInputPosition = currentInputPosition;
    }
    
    private void StopDragging()
    {
        isDragging = false;
    }
    
    private void SetRotation(float angle)
    {
        currentRotationAngle = angle;
        
        turretTransform.localRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);
        
        OnRotationChanged?.Invoke(currentRotationAngle);
        EventBus.Fire(new TurretRotationEvent(currentRotationAngle, weaponSettings.maxRotationAngle));
    }
    
    public void EnableControl()
    {
        controlEnabled = true;
    }
    
    public void DisableControl()
    {
        controlEnabled = false;
        isDragging = false;
    }
    
    public void ResetRotation()
    {
        SetRotation(0f);
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        OnRotationChanged = null;
    }
    
    // Debug info
    private void OnDrawGizmosSelected()
    {
        if (weaponSettings == null) return;
        
        Vector3 center = transform.position;
        Vector3 forward = transform.forward;
        
        Vector3 leftBound = Quaternion.Euler(0, -weaponSettings.maxRotationAngle, 0) * forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, leftBound * 3f);
        
        Vector3 rightBound = Quaternion.Euler(0, weaponSettings.maxRotationAngle, 0) * forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, rightBound * 3f);
        
        Vector3 currentDirection = Quaternion.Euler(0, currentRotationAngle, 0) * forward;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(center, currentDirection * 4f);
        
        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        }
    }
}