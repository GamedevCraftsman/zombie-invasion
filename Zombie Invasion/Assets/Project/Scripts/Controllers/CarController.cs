using System;
using System.Collections;
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
    //private bool _isStopping;
    private float _currentSpeed = 0;
    private float _lvlLength = 0;

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

    public void SmoothStop()
    {
        StartCoroutine(UpdateWinDeceleration());
    }
    
    private IEnumerator UpdateWinDeceleration()
    {
        // замінюємо null на WaitForFixedUpdate(), щоб корутина йшла в унісон з фізикою
        while (_currentSpeed > 0f)
        {
            _currentSpeed = Mathf.MoveTowards(
                _currentSpeed,
                0f,
                _carSettings.Deceleration * Time.fixedDeltaTime
            );
            yield return null;
        }

        // потроху вирівнюємо позицію без різкого «стрибка»
        float startZ = carTransform.position.z;
        float distanceToCover = _lvlLength - startZ;
        float t = 0f;
        float duration = distanceToCover / (_carSettings.Deceleration * Time.fixedDeltaTime); 
        // або просто кількість ітерацій, але тут краще час
        while (t < 1f)
        {
            t += Time.fixedDeltaTime / duration;
            float z = Mathf.Lerp(startZ, _lvlLength, t);
            carTransform.position = new Vector3(carTransform.position.x, 
                carTransform.position.y, 
                z);
            yield return null;
        }

        // наприкінці ставимо точно
        carTransform.position = new Vector3(
            carTransform.position.x,
            carTransform.position.y,
            _lvlLength
        );
        
        // фіксуємо позицію точно в кінці рівня
        Vector3 pos = carTransform.position;
        carTransform.position = new Vector3(pos.x, pos.y, _lvlLength);
        
        Debug.LogWarning("Real stop!");
        StopMovement();
        _isGameActive = false;
        //_gameManager.EndGame(true);
        
        
        /*while (_currentSpeed > 0)
        {
            _currentSpeed = Mathf.MoveTowards(
                _currentSpeed,
                0f,
                _carSettings.Deceleration * Time.fixedDeltaTime
            );
            
            yield return null;
        }
        
        StopMovement();
        _isGameActive = false;*/
    }
    public void ResetPosition()
         {
             carTransform.position = _carSettings.CarStartPosition;
             ResetCarState();
         }
    #endregion
    
     // public void LvlLenghtCalculation()
     // {
     //     //Round to the nearest tenth.
     //     _lvlLength = Mathf.Round((carTransform.position.z 
     //                               + (_gameSettings.MapLength - 1) 
     //                               * _gameSettings.DistanceBetweenTiles) * 10f) / 10f; 
     // }

    private void CheckLevelCompletion()
    {
        
        // якщо ми вже почали гальмувати — нічого не робимо
        //if (_isStopping) return;

        // відстань до фінішу
        float distanceToFinish = _lvlLength - carTransform.position.z;

        // обчислюємо гальмівний шлях: v^2 / (2 * a)
        float stoppingDistance = (_currentSpeed * _currentSpeed) 
                                 / (2f * _carSettings.Deceleration);

        //Debug.LogWarning($"Distance to finish: {distanceToFinish}/nStopping distance: {stoppingDistance}");
        // як тільки залишилося рівно стільки, щоб загальмувати — стартуємо корутину
        if (distanceToFinish <= stoppingDistance)
        {
            Debug.LogWarning("Stopping car");
            //_isStopping = true;
            _gameManager.EndGame(true);
            //StartCoroutine(UpdateWinDeceleration());
        }
        
        // if (carTransform.position.z >= _lvlLength)
        // {
        //     _gameManager.EndGame(true);
        // }
    }
}