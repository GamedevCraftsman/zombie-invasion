using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CarController : BaseController
{
    [Header("References")]
    [SerializeField] private Transform carTransform;
    
    // Dependencies
    [Inject] private CarSettings _carSettings;
    [Inject] private GameSettings _gameSettings;
    [Inject] private IGameManager _gameManager;
    
    // State
    private bool _isMoving = false;
    private bool _isGameActive = false;
    private float _currentSpeed = 0f;
    private bool _isWinning = false;
    private float _lvlLength = 0;
    
    // Properties for external access
    public bool IsMoving => _isMoving;
    public float CurrentSpeed => _currentSpeed;
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
        EventBus.Subscribe<CarReachedEndEvent>(OnReachedEndGame);
        EventBus.Subscribe<ContinueGameEvent>(OnContinueGame);
        EventBus.Subscribe<RestarGameEvent>(OnRestartGame);
    }
    
    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStarted);
        EventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus?.Unsubscribe<CarReachedEndEvent>(OnReachedEndGame);
        EventBus?.Unsubscribe<ContinueGameEvent>(OnContinueGame);
        EventBus?.Unsubscribe<RestarGameEvent>(OnRestartGame);
    }
    
    private void ResetCarState()
    {
        _lvlLength = 0f;
        _currentSpeed = 0f;
        _isMoving = false;
        _isGameActive = false;
        _isWinning = false;
    }
    
    private void OnGameStarted(StartGameEvent startEvent)
    {
        StartMovement();
        LvlLenghtCalculation();
    }

    private void OnRestartGame(RestarGameEvent restartEvent)
    {
        ResetPosition();
    }

    private void OnReachedEndGame(CarReachedEndEvent carReachedEndEvent)
    {
        //StartWinDeceleration();
        UpdateWinDeceleration();
        //StopMovement();
        
        _isGameActive = false;
    }
    
    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        // if(!gameOverEvent.IsWin)
        // {
            StopMovement();
        //}
        
        _isGameActive = false;
    }

    private void OnContinueGame(ContinueGameEvent continueEvent)
    {
        ResetCarState();
    }
    
    private void LvlLenghtCalculation()
    {
        _lvlLength = carTransform.position.z + (_gameSettings.MapLength - 1) * _gameSettings.MapTilePrefab.transform.localScale.y /*+ (_gameSettings.MapTilePrefab.transform.localScale.y / 2)*/; 
        Debug.LogWarning(_lvlLength);
    }
    
    private void StartMovement()
    {
        _isMoving = true;
        _isGameActive = true;
        _isWinning = false;
        _currentSpeed = 0f;
    }
    
    private void StopMovement()
    {
        _isMoving = false;
        _currentSpeed = 0f;
    }
    
    private void StartWinDeceleration()
    {
        _isWinning = true;
        // Залишаємо isMoving = true для плавного сповільнення
    }
    
    private void FixedUpdate()
    {
        if (!_isMoving) return;
        
        // if (_isWinning)
        // {
        //     UpdateWinDeceleration();
        // }
        if (_isGameActive)
        {
            UpdateNormalMovement();
            CheckLevelCompletion();
        }
    }
    
    private void UpdateNormalMovement()
    {
        // Плавне прискорення до цільової швидкості
        _currentSpeed = Mathf.MoveTowards(
            _currentSpeed, 
            _carSettings.Speed, 
            _carSettings.Acceleration * Time.fixedDeltaTime
        );
        
        // Застосування руху
        Vector3 movement = Vector3.forward * _currentSpeed * Time.fixedDeltaTime;
        carTransform.position += movement;
    }
    
    private void UpdateWinDeceleration()
    {
        // Плавне сповільнення після перемоги
        _currentSpeed = Mathf.MoveTowards(
            _currentSpeed, 
            0f, 
            _carSettings.Deceleration * Time.fixedDeltaTime
        );
        
        // Застосування руху
        Vector3 movement = Vector3.forward * _currentSpeed * Time.fixedDeltaTime;
        carTransform.position += movement;
        
        // Зупинка, коли швидкість дійшла до нуля
        if (_currentSpeed <= 0f)
        {
            StopMovement();
        }
    }
    
    private void CheckLevelCompletion()
    {
        if (carTransform.position.z >= _lvlLength)
        {
            //EventBus.Fire(new CarReachedEndEvent());
            _gameManager.EndGame(true);
        }
    }
    
    // Public methods для зовнішнього контролю
    private void ResetPosition()
    {
        carTransform.position = Vector3.zero;
        ResetCarState();
    }
    
    // public void SetSpeed(float newSpeed)
    // {
    //     if (_isGameActive && !_isWinning)
    //     {
    //         _currentSpeed = Mathf.Clamp(newSpeed, 0f, _carSettings.Speed);
    //     }
    // }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}