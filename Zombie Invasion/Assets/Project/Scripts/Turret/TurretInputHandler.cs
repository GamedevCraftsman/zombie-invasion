using UnityEngine;

public class TurretInputHandler : ITurretInputHandler
{
    private readonly WeaponSettings _weaponSettings; 
    private readonly IInputController _inputController;
    private readonly ITurretController _turretController;
    private readonly IFireController _fireController;
    
    private bool _isDragging;
    private Vector2 _lastInputPosition;

    public TurretInputHandler(ITurretController turretController, WeaponSettings weaponSettings, IInputController inputController, IFireController fireController)
    {
        _turretController = turretController;
        _weaponSettings = weaponSettings;
        _inputController = inputController;
        _fireController = fireController;
        
        Initialize();
    }

    private void Initialize()
    {
        _turretController.OnGamePlaying += HandleInput;
        _turretController.OnGamePlaying += HandleShooting;
    }
    
    private void HandleShooting()
    {
        if (_isDragging && _fireController.CanFire())
        {
            _fireController.Fire();
        }
    }

    #region Turret Movement
    private void HandleInput()
    {
        InputType inputType = _inputController.LastInputType;

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
        else if (Input.GetMouseButton(0) && _isDragging)
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
                case TouchPhase.Moved when _isDragging:
                    ContinueDragging(touch.position);
                    break;
                case TouchPhase.Ended:
                    StopDragging();
                    break;
                case TouchPhase.Canceled:
                    StopDragging();
                    break;
            }
        }
    }
    
    private void StartDragging(Vector2 inputPosition)
    {
        _isDragging = true;
        _lastInputPosition = inputPosition;
    }

    private void ContinueDragging(Vector2 currentInputPosition)
    {
        if (!_isDragging) return;

        Vector2 deltaPosition = currentInputPosition - _lastInputPosition;
        float horizontalDelta = deltaPosition.x * _weaponSettings.InputSensitivity;

        // Прибираємо 0.01f - він робить рух залежним від FPS
        float rotationDelta = horizontalDelta * _weaponSettings.RotationSpeed * Time.deltaTime;
        float newAngle = _turretController.CurrentRotationAngle + rotationDelta;

        newAngle = Mathf.Clamp(newAngle, -_weaponSettings.MaxRotationAngle, _weaponSettings.MaxRotationAngle);

        _turretController.SetRotation(newAngle);
    
        _lastInputPosition = currentInputPosition;
    }

    public void StopDragging()
    {
        _isDragging = false;
    }
    #endregion
}
