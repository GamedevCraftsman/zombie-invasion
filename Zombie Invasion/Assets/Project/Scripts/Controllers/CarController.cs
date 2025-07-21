using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CarController : BaseController, ICarController
{
    [Header("References")] [SerializeField]
    private Transform carTransform;

    // Dependencies
    private CarSettings _carSettings;
    private GameSettings _gameSettings;
    private IGameManager _gameManager;

    // State
    private bool _isMoving;
    private bool _isGameActive;
    private float _currentSpeed;
    private float _lvlLength;

    public Transform CarTransform => carTransform;
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
            if (carTransform == null)
                carTransform = transform;

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
        _lvlLength = _gameSettings.LvlLenghtCalculation(carTransform);
        
        _isMoving = true;
        _isGameActive = true;
        _currentSpeed = 0f;
    }

    public void StopMovement()
    {
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
        carTransform.position += movement;
    }
    
    public void ResetPosition()
         {
             carTransform.position = _carSettings.CarStartPosition;
             ResetCarState();
         }
    #endregion

    private void CheckLevelCompletion()
    {
        if (carTransform.position.z >= _lvlLength)
        {
            _gameManager.EndGame(true);
            SetCorrectPosition();
        }
    }

    private void SetCorrectPosition()
    {
        Vector3 pos = carTransform.position;
        carTransform.position = new Vector3(pos.x, pos.y, _lvlLength);
    }
}