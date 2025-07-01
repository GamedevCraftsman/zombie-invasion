using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class TurretController : BaseController, ITurretController
{
    
    [SerializeField] private Transform turretTransform;
    
    //State
    //private bool controlEnabled = false;
    private float currentRotationAngle = 0f;
    private IEnumerator _checkInput;

    // Shooting state
 

    // Properties
    //public bool IsControlEnabled => controlEnabled;
    public float CurrentRotationAngle => currentRotationAngle;

    // Events
    //public event Action<float> OnRotationChanged;
    public event Action OnGamePlaying;
    

    protected override Task Initialize()
    {
        try
        {
            ValidateComponents();
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
        /*if (turretTransform == null)
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
        */

        // if (_weaponSettings == null)
        // {
        //     Debug.LogError("WeaponSettings is null!");
        // }
        
        _checkInput = CheckInput();
    }
    
    private IEnumerator CheckInput()
    {
        //if (_weaponSettings == null) yield break;
        
        while (true)
        {
            OnGamePlaying?.Invoke();
            yield return null;
        }
    }
    
    
   
    public void SetRotation(float angle)
    {
        currentRotationAngle = angle;

        turretTransform.localRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);
        
        //OnRotationChanged?.Invoke(currentRotationAngle);
        //EventBus.Fire(new TurretRotationEvent(currentRotationAngle, _weaponSettings.MaxRotationAngle));
    }

    public void EnableControl()
    {
        //controlEnabled = true;
        StartCoroutine(_checkInput);
        Debug.Log("EnableControl");
    }

    public void DisableControl()
    {
        //controlEnabled = false;
        StopCoroutine(_checkInput);
        Debug.Log("DisableControl");
    }

    public void ResetRotation()
    {
        SetRotation(0f);
    }

    private void OnDestroy()
    {
        OnGamePlaying = null;
        //OnRotationChanged = null;
    }
}