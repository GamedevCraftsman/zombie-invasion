using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CarHPUIController : BaseController, ICarHPUIController
{
    [Header("UI References")] 
    [SerializeField] private Image hpFillImage;
    [SerializeField] private CanvasGroup hpBarGroup;
    [Header("Visual Settings")] 
    [SerializeField] private float animationSpeed = 5f;

    private Tween _showHpTween;
    private GameplayUISettings _gameplayUISettings;
    private float _targetFillAmount;
    private float _currentFillAmount;
    private bool _isUpdateHpRunning;

    [Inject]
    public void Construct(GameplayUISettings gameplayUISettings)
    {
        _gameplayUISettings = gameplayUISettings;
    }

    protected override Task Initialize()
    {
        try
        {
            if (hpFillImage == null)
            {
                Debug.LogError("HpUIController: HP Fill Image is missing!");
            }

            ResetUI();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return Task.CompletedTask;
    }

    public void ShowHpBar()
    {
        _showHpTween = hpBarGroup.DOFade(_gameplayUISettings.HpBarEndFadeLvl, _gameplayUISettings.HpBarShowDuration);
    }

    public void HideHpBar()
    {
        _showHpTween.Kill();
        
        hpBarGroup.alpha = 0;
    }

    public void ResetUI()
    {
        _targetFillAmount = _gameplayUISettings.TargetFillAmount;
        _currentFillAmount = _gameplayUISettings.TargetFillAmount;

        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = _gameplayUISettings.TargetFillAmount;
        }
    }

    public void UpdateHp(float hpPercentage, int currentHp, int maxHp)
    {
        _targetFillAmount = Mathf.Clamp01(hpPercentage);

        if (_isUpdateHpRunning) return;
        StartCoroutine(SmoothDecrease());

        Debug.Log($"HP UI оновлено: {currentHp}/{maxHp} ({{hpPercentage:P0}})");
    }

    private IEnumerator SmoothDecrease()
    {
        _isUpdateHpRunning = true;

        while (Mathf.Abs(_currentFillAmount - _targetFillAmount) > 0.01f)
        {
            Decrease();
            ApplyVisualChanges();
            yield return null;
        }

        _isUpdateHpRunning = false;
    }

    private void Decrease()
    {
        _currentFillAmount = Mathf.MoveTowards(
            _currentFillAmount,
            _targetFillAmount,
            animationSpeed * Time.deltaTime
        );
    }

    private void ApplyVisualChanges()
    {
        if (hpFillImage == null) return;

        hpFillImage.fillAmount = _currentFillAmount;
    }
}