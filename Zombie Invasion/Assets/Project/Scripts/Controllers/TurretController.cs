using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class TurretController : BaseController, ITurretController
{
    [SerializeField] private Transform turretTransform;
    
    private IEnumerator _checkInput;
    public event Action OnGamePlaying;
    
    protected override Task Initialize()
    {
        try
        {
            SetComponents();
            ResetRotation();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return Task.CompletedTask;
    }

    private void SetComponents()
    {
        _checkInput = CheckInput();
    }
    
    private IEnumerator CheckInput()
    {
        while (true)
        {
            OnGamePlaying?.Invoke();
            yield return null;
        }
    }
   
    public void SetRotation(float angle)
    {
        turretTransform.localRotation = Quaternion.Euler(0f, angle, 0f);
    }

    public void EnableControl()
    {
        StartCoroutine(_checkInput);
        Debug.Log("EnableControl");
    }

    public void DisableControl()
    {
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
    }
}