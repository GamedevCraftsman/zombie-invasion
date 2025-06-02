using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CarHPUIController : BaseController
{
    [Header("UI References")] [SerializeField]
    private Image hpFillImage;

    [SerializeField] private CanvasGroup hpBarGroup;

    [Header("Visual Settings")] [SerializeField]
    private float animationSpeed = 5f;

    [SerializeField] private bool useSmoothing = true;

    // State
    private float targetFillAmount = 1f;
    private float currentFillAmount = 1f;

    protected override Task Initialize()
    {
        if (hpFillImage == null)
        {
            Debug.LogError("HPUIController: HP Fill Image is missing!");
            return Task.CompletedTask;
        }

        ResetUI();
        SubscribeToEvents();

        return Task.CompletedTask;
    }

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<HPChangedEvent>(OnHPChanged);
        EventBus.Subscribe<StartGameEvent>(OnGameStarted);
        EventBus.Subscribe<GameOverEvent>(OnGameEnd);
        EventBus.Subscribe<CarReachedEndEvent>(OnGameEnd);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<GameOverEvent>(OnGameEnd);
        EventBus.Unsubscribe<CarReachedEndEvent>(OnGameEnd);
        EventBus?.Unsubscribe<HPChangedEvent>(OnHPChanged);
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStarted);
    }

    private void OnGameStarted(StartGameEvent startEvent)
    {
        ResetUI();
        ShowHPBar();
    }

    private void OnGameEnd(GameOverEvent gameOverEvent)
    {
        HideHPBar();
    }

    private void OnGameEnd(CarReachedEndEvent carReachedEndEvent)
    {
        HideHPBar();
    }

    private void ShowHPBar()
    {
        hpBarGroup.DOFade(1, 2);
    }

    private void HideHPBar()
    {
        hpBarGroup.alpha = 0;
    }

    private void OnHPChanged(HPChangedEvent hpEvent)
    {
        UpdateHP(hpEvent.HPPercentage, hpEvent.CurrentHP, hpEvent.MaxHP);
    }

    private void ResetUI()
    {
        targetFillAmount = 1f;
        currentFillAmount = 1f;

        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = 1f;
        }
    }

    private void UpdateHP(float hpPercentage, int currentHP, int maxHP)
    {
        targetFillAmount = Mathf.Clamp01(hpPercentage);

        if (!useSmoothing)
        {
            currentFillAmount = targetFillAmount;
            ApplyVisualChanges();
        }

        Debug.Log($"HP UI оновлено: {currentHP}/{maxHP} ({hpPercentage:P0})");
    }

    private void Update()
    {
        if (!useSmoothing) return;

        SmoothDecrease();
    }

    private void SmoothDecrease()
    {
        if (Mathf.Abs(currentFillAmount - targetFillAmount) > 0.01f)
        {
            currentFillAmount = Mathf.MoveTowards(
                currentFillAmount,
                targetFillAmount,
                animationSpeed * Time.deltaTime
            );

            ApplyVisualChanges();
        }
    }

    private void ApplyVisualChanges()
    {
        if (hpFillImage == null) return;

        hpFillImage.fillAmount = currentFillAmount;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}