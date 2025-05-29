using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class TurretController : BaseController, ITurretController
{
    [Header("References")]
    [SerializeField] private Transform turretTransform;
    
    [Header("Settings")]
    [SerializeField] private WeaponSettings weaponSettings;
    
    // Injected dependencies
    [Inject] private IInputController inputController;
    
    // State
    private bool controlEnabled = false;
    private float currentRotationAngle = 0f;
    private bool isDragging = false;
    private Vector2 lastInputPosition;
    
    // Properties
    public bool IsControlEnabled => controlEnabled;
    public float CurrentRotationAngle => currentRotationAngle;
    
    // Events
    public event Action<float> OnRotationChanged;
    
    protected override Task Initialize()
    {
        ValidateComponents();
        SubscribeToEvents();
        ResetRotation();
        
        Debug.Log("TurretController ініціалізовано");
        return Task.CompletedTask;
    }
    
    private void ValidateComponents()
    {
        if (turretTransform == null)
        {
            turretTransform = transform;
            Debug.LogWarning("TurretTransform не встановлено, використовується поточний transform");
        }
        
        if (weaponSettings == null)
        {
            Debug.LogError("WeaponSettings не встановлено!");
        }
    }
    
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<StartGameEvent>(OnGameStarted);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
    }
    
    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStarted);
        EventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
    }
    
    private void OnGameStarted(StartGameEvent gameStartedEvent)
    {
        EnableControl();
        Debug.Log("Турель активована після початку гри");
    }
    
    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        DisableControl();
        ResetRotation();
        Debug.Log("Турель деактивована після закінчення гри");
    }
    
    private void Update()
    {
        if (!controlEnabled || weaponSettings == null) return;
        
        HandleInput();
    }
    
    private void HandleInput()
    {
        // Перевіряємо тип останнього вводу
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
        
        // Обчислюємо різницю в позиції
        Vector2 deltaPosition = currentInputPosition - lastInputPosition;
        
        // Використовуємо горизонтальну різницю для обертання
        float horizontalDelta = deltaPosition.x * weaponSettings.inputSensitivity;
        
        // Обчислюємо новий кут
        float rotationDelta = horizontalDelta * weaponSettings.rotationSpeed * Time.deltaTime * 0.01f;
        float newAngle = currentRotationAngle + rotationDelta;
        
        // Обмежуємо кут поворот
        newAngle = Mathf.Clamp(newAngle, -weaponSettings.maxRotationAngle, weaponSettings.maxRotationAngle);
        
        // Застосовуємо обертання
        SetRotation(newAngle);
        
        // Оновлюємо останню позицію
        lastInputPosition = currentInputPosition;
    }
    
    private void StopDragging()
    {
        isDragging = false;
    }
    
    private void SetRotation(float angle)
    {
        currentRotationAngle = angle;
        
        // Застосовуємо обертання до transform
        turretTransform.localRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);
        
        // Викликаємо events
        OnRotationChanged?.Invoke(currentRotationAngle);
        EventBus.Fire(new TurretRotationEvent(currentRotationAngle, weaponSettings.maxRotationAngle));
    }
    
    // ITurretController implementation
    public void EnableControl()
    {
        controlEnabled = true;
        Debug.Log("Управління турелью увімкнено");
    }
    
    public void DisableControl()
    {
        controlEnabled = false;
        isDragging = false;
        Debug.Log("Управління турелью вимкнено");
    }
    
    public void ResetRotation()
    {
        SetRotation(0f);
        Debug.Log("Поворот турелі скинуто");
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
        
        // Малюємо межі повороту
        Vector3 center = transform.position;
        Vector3 forward = transform.forward;
        
        // Ліва межа
        Vector3 leftBound = Quaternion.Euler(0, -weaponSettings.maxRotationAngle, 0) * forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, leftBound * 3f);
        
        // Права межа
        Vector3 rightBound = Quaternion.Euler(0, weaponSettings.maxRotationAngle, 0) * forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, rightBound * 3f);
        
        // Поточний напрямок
        Vector3 currentDirection = Quaternion.Euler(0, currentRotationAngle, 0) * forward;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(center, currentDirection * 4f);
    }
}