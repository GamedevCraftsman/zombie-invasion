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

    // Dependencies
    private CarSettings _carSettings;
    private GameSettings _gameSettings;
    private IGameManager _gameManager;

    // State
    private bool _isMoving;
    private bool _isGameActive;
    private float _currentSpeed;
    private float _lvlLength;

    public GameObject Car => car;
    [Inject]
    public void Construct(CarSettings carSettings, GameSettings gameSettings, IGameManager gameManager)
    {
        _carSettings = carSettings;
        _gameSettings = gameSettings;
        _gameManager = gameManager;
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
        _lvlLength = _gameSettings.LvlLenghtCalculation(car.transform);
        wheelRotator.StartRotating(_carSettings.Speed);
        
        _isMoving = true;
        _isGameActive = true;
        _currentSpeed = 0f;
    }

    public void StopMovement()
    {
        wheelRotator.StopRotating();
        
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

    private void SetCorrectPosition()
    {
        Vector3 pos = car.transform.position;
        car.transform.position = new Vector3(pos.x, pos.y, _lvlLength);
    }
}