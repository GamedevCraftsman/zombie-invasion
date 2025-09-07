using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class CarController : BaseController, ICarController
{
    [Header("References")] 
    [SerializeField] private GameObject car;
    [Header("Additional")]
    [SerializeField] private WheelRotator wheelRotator;
    [FormerlySerializedAs("particles")] [SerializeField] private ParticleSystem wheelDust;

    // Dependencies
    private CarSettings _carSettings;
    private GameSettings _gameSettings;
    private IGameManager _gameManager;
    private IProgressIncreaseService _progressIncreaseService;
    private ICheckpointTileService _checkpointTileService;
    
    // State
    private bool _isMoving;
    private bool _isGameActive;
    private float _currentSpeed;
    private float _lvlLength;
    private float _distanceToMoveCheckpoint;

    public GameObject Car => car;
    [Inject]
    public void Construct(CarSettings carSettings, GameSettings gameSettings, IGameManager gameManager, IProgressIncreaseService progressIncreaseService
    , ICheckpointTileService checkpointTileService)
    {
        _carSettings = carSettings;
        _gameSettings = gameSettings;
        _gameManager = gameManager;
        _progressIncreaseService = progressIncreaseService;
        _checkpointTileService = checkpointTileService;
    }
    
    protected override async Task Initialize()
    {
        try
        {
            ResetCarState();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        await Task.CompletedTask;
    }

    private void FixedUpdate()
    {
        if (!_isMoving) return;

        if (_isGameActive)
        {
            UpdateNormalMovement();
            CheckLevelCompletion();
            CheckMoveCheckpoint();
        }
    }
   
    #region States
    public void ResetCarState()
    {
        _lvlLength = 0f;
        _currentSpeed = 0f;
        _isMoving = false;
        _isGameActive = false;
    }

    public void StartMovement()
    {
        _lvlLength = LvlLenghtCalculation();
        _distanceToMoveCheckpoint = DistanceToMoveCheckpointCalculation();
        
        wheelRotator.StartRotating(_carSettings.Speed);
        wheelDust.Play();
        
        _isMoving = true;
        _isGameActive = true;
        _currentSpeed = 0f;
    }

    private float LvlLenghtCalculation()
    {
        //Round to the nearest tenth.
        float lvlLenght = Mathf.Round((car.transform.position.z 
                                       + (_gameSettings.MapLength + 2 + _progressIncreaseService.MapIncrease - 1) 
                                       * _gameSettings.DistanceBetweenTiles) * 10f) / 10f; 
        
        return lvlLenght;
    }

    private float DistanceToMoveCheckpointCalculation()
    {
        float distance = Mathf.Round((car.transform.position.z 
                                      + _gameSettings.DistanceBetweenTiles) * 10f) / 10f; 
        return distance;
    }
    
    public void StopMovement()
    {
        wheelRotator.StopRotating();
        wheelDust.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        
        _isMoving = false;
        _isGameActive = false;
        _currentSpeed = 0f;
    }
    #endregion
    #region Movement
    private void UpdateNormalMovement()
    {
        if (!Mathf.Approximately(_currentSpeed, _carSettings.Speed))
        {
            _currentSpeed = Mathf.MoveTowards(
                _currentSpeed,
                _carSettings.Speed,
                _carSettings.Acceleration * Time.fixedDeltaTime
            );
        }

        Move();
    }

    private void Move()
    {
        Vector3 movement = Vector3.forward * (_currentSpeed * Time.fixedDeltaTime);
        car.transform.position += movement;
    }
    
    public void ResetPosition()
    {
        car.transform.position = _carSettings.CarStartPosition;
        ResetCarState();
    }
    #endregion

    private void CheckLevelCompletion()
    {
        if (car.transform.position.z >= _lvlLength)
        {
            _gameManager.EndGame(true);
            SetCorrectPosition();
        }
    }

    private void CheckMoveCheckpoint()
    {
        if (car.transform.position.z >= _distanceToMoveCheckpoint)
        {
            Vector3 pos = car.transform.position;
            _checkpointTileService.MoveTile(new Vector3(pos.x, pos.y, _lvlLength));
        }
    }
    
    private void SetCorrectPosition()
    {
        Vector3 pos = car.transform.position;
        car.transform.position = new Vector3(pos.x, pos.y, _lvlLength);
    }
}