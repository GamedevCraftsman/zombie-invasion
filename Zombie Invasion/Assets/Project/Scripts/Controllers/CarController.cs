using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CarController : BaseController
{
    [Header("References")]
    [SerializeField] private Transform carTransform;
    
    // Dependencies
    [Inject] private CarSettings carSettings;
    [Inject] private GameSettings gameSettings;
    
    // State
    private bool isMoving = false;
    private bool isGameActive = false;
    private float currentSpeed = 0f;
    private bool isWinning = false;
    
    // Properties for external access
    public bool IsMoving => isMoving;
    public float CurrentSpeed => currentSpeed;
    public Vector3 Position => carTransform.position;
    
    protected override async Task Initialize()
    {
        if (carTransform == null)
            carTransform = transform;
            
        SubscribeToEvents();
        ResetCarState();
        
        await Task.CompletedTask;
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
    
    private void ResetCarState()
    {
        carTransform.position = Vector3.zero;
        currentSpeed = 0f;
        isMoving = false;
        isGameActive = false;
        isWinning = false;
    }
    
    private void OnGameStarted(StartGameEvent startEvent)
    {
        StartMovement();
    }
    
    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        if (gameOverEvent.IsWin)
        {
            StartWinDeceleration();
        }
        else
        {
            StopMovement();
        }
        
        isGameActive = false;
    }
    
    private void StartMovement()
    {
        isMoving = true;
        isGameActive = true;
        isWinning = false;
        currentSpeed = 0f;
    }
    
    private void StopMovement()
    {
        isMoving = false;
        currentSpeed = 0f;
    }
    
    private void StartWinDeceleration()
    {
        isWinning = true;
        // Залишаємо isMoving = true для плавного сповільнення
    }
    
    private void FixedUpdate()
    {
        if (!isMoving) return;
        
        if (isWinning)
        {
            UpdateWinDeceleration();
        }
        else if (isGameActive)
        {
            UpdateNormalMovement();
            CheckLevelCompletion();
        }
    }
    
    private void UpdateNormalMovement()
    {
        // Плавне прискорення до цільової швидкості
        currentSpeed = Mathf.MoveTowards(
            currentSpeed, 
            carSettings.Speed, 
            carSettings.Acceleration * Time.fixedDeltaTime
        );
        
        // Застосування руху
        Vector3 movement = Vector3.forward * currentSpeed * Time.fixedDeltaTime;
        carTransform.position += movement;
    }
    
    private void UpdateWinDeceleration()
    {
        // Плавне сповільнення після перемоги
        currentSpeed = Mathf.MoveTowards(
            currentSpeed, 
            0f, 
            carSettings.Deceleration * Time.fixedDeltaTime
        );
        
        // Застосування руху
        Vector3 movement = Vector3.forward * currentSpeed * Time.fixedDeltaTime;
        carTransform.position += movement;
        
        // Зупинка, коли швидкість дійшла до нуля
        if (currentSpeed <= 0f)
        {
            StopMovement();
        }
    }
    
    private void CheckLevelCompletion()
    {
        if (carTransform.position.z >= gameSettings.MapLength)
        {
            EventBus.Fire(new CarReachedEndEvent());
        }
    }
    
    // Public methods для зовнішнього контролю
    public void ResetPosition()
    {
        ResetCarState();
    }
    
    public void SetSpeed(float newSpeed)
    {
        if (isGameActive && !isWinning)
        {
            currentSpeed = Mathf.Clamp(newSpeed, 0f, carSettings.Speed);
        }
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}