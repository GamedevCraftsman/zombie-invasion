using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HPUIController : BaseController
{
    [Header("UI References")]
    [SerializeField] private Image hpFillImage;
    [SerializeField] private Text hpText; // Опціонально для відображення цифр
    
    [Header("Visual Settings")]
    [SerializeField] private Color fullHPColor = Color.green;
    [SerializeField] private Color midHPColor = Color.yellow;
    [SerializeField] private Color lowHPColor = Color.red;
    [SerializeField] private float lowHPThreshold = 0.3f;
    [SerializeField] private float midHPThreshold = 0.6f;
    [SerializeField] private float animationSpeed = 5f;
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
        
        // Встановлюємо початкові значення
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
            hpFillImage.color = fullHPColor;
        }
        
        if (hpText != null)
        {
            hpText.text = "100/100";
        }
    }
    
    private void UpdateHP(float hpPercentage, int currentHP, int maxHP)
    {
        targetFillAmount = Mathf.Clamp01(hpPercentage);
        
        // Оновлюємо текст миттєво
        if (hpText != null)
        {
            hpText.text = $"{currentHP}/{maxHP}";
        }
        
        // Якщо не використовуємо згладжування, оновлюємо миттєво
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
        
        // Плавна анімація fill amount
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
        
        // Оновлюємо fill amount
        hpFillImage.fillAmount = currentFillAmount;
        
        // Оновлюємо колір залежно від рівня HP
        Color targetColor = GetHPColor(currentFillAmount);
        hpFillImage.color = targetColor;
    }
    
    private Color GetHPColor(float hpPercentage)
    {
        if (hpPercentage <= lowHPThreshold)
        {
            return lowHPColor;
        }
        else if (hpPercentage <= midHPThreshold)
        {
            // Градієнт між червоним та жовтим
            float t = (hpPercentage - lowHPThreshold) / (midHPThreshold - lowHPThreshold);
            return Color.Lerp(lowHPColor, midHPColor, t);
        }
        else
        {
            // Градієнт між жовтим та зеленим
            float t = (hpPercentage - midHPThreshold) / (1f - midHPThreshold);
            return Color.Lerp(midHPColor, fullHPColor, t);
        }
    }
    
    // Public methods для налаштування з коду
    public void SetColors(Color full, Color mid, Color low)
    {
        fullHPColor = full;
        midHPColor = mid;
        lowHPColor = low;
    }
    
    public void SetThresholds(float low, float mid)
    {
        lowHPThreshold = Mathf.Clamp01(low);
        midHPThreshold = Mathf.Clamp01(mid);
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