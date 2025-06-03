using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CarController : BaseController
{
    [Header("References")] [SerializeField]
    private Transform carTransform;

    // Dependencies
    [Inject] private CarSettings _carSettings;
    [Inject] private GameSettings _gameSettings;
    [Inject] private IGameManager _gameManager;

    // State
    private bool _isMoving = false;
    private bool _isGameActive = false;
    private float _currentSpeed = 0f;
    private float _lvlLength = 0;

    // Properties for external access
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
        UpdateWinDeceleration();
        _isGameActive = false;
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        StopMovement();
        _isGameActive = false;
    }

    private void OnContinueGame(ContinueGameEvent continueEvent)
    {
        ResetCarState();
    }

    private void LvlLenghtCalculation()
    {
        _lvlLength = Mathf.Round((carTransform.position.z +
                                  (_gameSettings.MapLength - 1) * _gameSettings.DistanceBetweenTiles) *
                                 10f) / 10f; 
    }

    private void StartMovement()
    {
        _isMoving = true;
        _isGameActive = true;
        _currentSpeed = 0f;
    }

    private void StopMovement()
    {
        _isMoving = false;
        _currentSpeed = 0f;
    }

    private void FixedUpdate()
    {
        if (!_isMoving) return;

        if (_isGameActive)
        {
            UpdateNormalMovement();
            CheckLevelCompletion();
        }
    }

    private void UpdateNormalMovement()
    {
        _currentSpeed = Mathf.MoveTowards(
            _currentSpeed,
            _carSettings.Speed,
            _carSettings.Acceleration * Time.fixedDeltaTime
        );

        Move();
    }

    private void Move()
    {
        Vector3 movement = Vector3.forward * _currentSpeed * Time.fixedDeltaTime;
        carTransform.position += movement;
    }

    private void UpdateWinDeceleration()
    {
        _currentSpeed = Mathf.MoveTowards(
            _currentSpeed,
            0f,
            _carSettings.Deceleration * Time.fixedDeltaTime
        );

        Move();

        if (_currentSpeed <= 0f)
        {
            StopMovement();
        }
    }

    private void CheckLevelCompletion()
    {
        if (carTransform.position.z >= _lvlLength)
        {
            _gameManager.EndGame(true);
        }
    }

    private void ResetPosition()
    {
        carTransform.position = _carSettings.CarStartPosition;
        ResetCarState();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}