using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class InputController : BaseController, IInputController
{
    [Inject] private IGameManager _gameManager;
    
    [Header("Input Settings")]
    [SerializeField] private bool enableKeyboardInput = true;
    [SerializeField] private bool enableMouseInput = true;
    [SerializeField] private bool enableTouchInput = true;
    [SerializeField] private KeyCode startGameKey = KeyCode.Space;
    
    // State
    private bool gameStarted = false;
    private bool inputEnabled = true;
    private InputType lastInputType = InputType.None;
    
    // Properties
    //public bool IsInputEnabled => inputEnabled;
   // public bool IsGameStarted => gameStarted;
   public bool IsInputEnabled { get; }
   public bool IsGameStarted { get; }
   public InputType LastInputType => lastInputType;
    
    // Events
    //public event Action OnGameStartRequested;

    //public event Action<InputType> OnInputDetected;
    //public event Action<InputType> OnInputDetected;
    
    protected override Task Initialize()
    {
        SubscribeToEvents();
        ResetInputState();
        
        Debug.Log("InputController ініціалізовано");
        return Task.CompletedTask;
    }
    
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
    }
    
    private void UnsubscribeFromEvents()
    {
        EventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
    }
    
    private void ResetInputState()
    {
        gameStarted = false;
        inputEnabled = true;
        lastInputType = InputType.None;
    }
    
    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        ResetInputState();
        Debug.Log("InputController готовий до нової гри");
    }
    
    private void Update()
    {
       // if (!inputEnabled || gameStarted) return;
        
        InputType detectedInput = DetectInput();
       
        if (detectedInput != InputType.None && _gameManager.CurrentState == GameState.Menu)
        {
            lastInputType = detectedInput;
            //OnInputDetected?.Invoke(detectedInput);
            StartGame();
        }
    }
    
    private InputType DetectInput()
    {
        // Перевірка клавіатурного вводу
        if (enableKeyboardInput && Input.GetKeyDown(startGameKey))
        {
            Debug.Log($"Гра запущена через клавішу {startGameKey}");
            return InputType.Keyboard;
        }
        
        // Перевірка миші
        if (enableMouseInput && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Гра запущена через клік миші");
            return InputType.Mouse;
        }
        
        // Перевірка дотику (для мобільних)
        if (enableTouchInput && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Гра запущена через дотик");
                return InputType.Touch;
            }
        }
        
        return InputType.None;
    }
    
    private void StartGame()
    {
        // gameStarted = true;
        // inputEnabled = false;
        // Викликаємо event для підписників
        //OnGameStartRequested?.Invoke();
        
        // Відправляємо через EventBus для backward compatibility
        
        EventBus.Fire(new ReadyGameEvent());
        Debug.Log($"🚀 StartGameEvent відправлено через {lastInputType}!");
    }
    
    // IInputController implementation
    public void EnableInput()
    {
        inputEnabled = true;
        Debug.Log("Input увімкнено");
    }
    
    public void DisableInput()
    {
        inputEnabled = false;
        Debug.Log("Input вимкнено");
    }
    
    public void ResetForNewGame()
    {
        ResetInputState();
        Debug.Log("InputController скинуто для нової гри");
    }
    
    public void SetStartGameKey(KeyCode newKey)
    {
        startGameKey = newKey;
        Debug.Log($"Клавіша запуску змінена на {newKey}");
    }
    
    public void EnableKeyboardInput(bool enable)
    {
        enableKeyboardInput = enable;
        Debug.Log($"Клавіатурний ввід: {(enable ? "увімкнено" : "вимкнено")}");
    }
    
    public void EnableMouseInput(bool enable)
    {
        enableMouseInput = enable;
        Debug.Log($"Ввід миші: {(enable ? "увімкнено" : "вимкнено")}");
    }
    
    public void EnableTouchInput(bool enable)
    {
        enableTouchInput = enable;
        Debug.Log($"Сенсорний ввід: {(enable ? "увімкнено" : "вимкнено")}");
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        
        // Очищуємо events
        //OnGameStartRequested = null;
        //OnInputDetected = null;
    }
}