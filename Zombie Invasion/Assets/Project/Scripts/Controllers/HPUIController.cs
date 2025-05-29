using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HPUIController : BaseController
{
    [Header("UI References")] [SerializeField]
    private Image hpFillImage;

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
            Debug.LogError("HPUIController: HP Fill Image не призначено!");
            return Task.CompletedTask;
        }

        ResetUI();
        SubscribeToEvents();

        Debug.Log("HPUIController ініціалізовано");
        return Task.CompletedTask;
    }

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<HPChangedEvent>(OnHPChanged);
        EventBus.Subscribe<StartGameEvent>(OnGameStarted);
    }

    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<HPChangedEvent>(OnHPChanged);
        EventBus?.Unsubscribe<StartGameEvent>(OnGameStarted);
    }

    private void OnGameStarted(StartGameEvent startEvent)
    {
        ResetUI();
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

    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = Mathf.Max(0f, speed);
    }

    public void EnableSmoothing(bool enable)
    {
        useSmoothing = enable;

        if (!enable)
        {
            currentFillAmount = targetFillAmount;
            ApplyVisualChanges();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}