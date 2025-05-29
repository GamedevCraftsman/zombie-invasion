using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CarControllerTester : MonoBehaviour
{
    [Inject] private IEventBus eventBus;
    [Inject] private CarController carController;
    
    [Header("Test Settings")]
    [SerializeField] private KeyCode startGameKey = KeyCode.Space;
    [SerializeField] private KeyCode damageTestKey = KeyCode.D;
    [SerializeField] private KeyCode winTestKey = KeyCode.W;
    [SerializeField] private KeyCode loseTestKey = KeyCode.L;
    
    private bool gameStarted = false;
    
    private void Start()
    {
        // Ініціалізуємо CarController
        //await carController.InitializeAsync();
        
        // Підписуємося на події для логування
        eventBus.Subscribe<StartGameEvent>(OnStartGame);
        eventBus.Subscribe<CarReachedEndEvent>(OnCarReachedEnd);
        eventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        eventBus.Subscribe<GameOverEvent>(OnGameOver);
        
        Debug.Log("CarController Tester готовий!");
        Debug.Log($"Натисніть {startGameKey} для початку гри");
        Debug.Log($"Натисніть {damageTestKey} для тесту урону");
        Debug.Log($"Натисніть {winTestKey} для тесту перемоги");
        Debug.Log($"Натисніть {loseTestKey} для тесту поразки");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(startGameKey) && !gameStarted)
        {
            StartGame();
        }
        
        if (Input.GetKeyDown(damageTestKey))
        {
            TestDamage();
        }
        
        if (Input.GetKeyDown(winTestKey))
        {
            TestWin();
        }
        
        if (Input.GetKeyDown(loseTestKey))
        {
            TestLose();
        }
        
        // Тест тапу по екрану (для мобільних)
        if (Input.GetMouseButtonDown(0) && !gameStarted)
        {
            StartGame();
        }
    }
    
    private void StartGame()
    {
        Debug.Log("🚀 Запуск гри через тестер!");
        eventBus.Fire(new StartGameEvent());
        gameStarted = true;
    }
    
    private void TestDamage()
    {
        Debug.Log("💥 Тест урону: 25 HP");
        eventBus.Fire(new PlayerDamagedEvent(25));
    }
    
    private void TestWin()
    {
        Debug.Log("🎉 Тест перемоги!");
        eventBus.Fire(new GameOverEvent(true));
        gameStarted = false;
    }
    
    private void TestLose()
    {
        Debug.Log("💀 Тест поразки!");
        eventBus.Fire(new GameOverEvent(false));
        gameStarted = false;
    }
    
    // Event handlers для логування
    private void OnStartGame(StartGameEvent e)
    {
        Debug.Log("✅ StartGameEvent отримано!");
        Debug.Log($"Машина рухається: {carController.IsMoving}");
    }
    
    private void OnCarReachedEnd(CarReachedEndEvent e)
    {
        Debug.Log("🏁 CarReachedEndEvent отримано!");
    }
    
    private void OnPlayerDamaged(PlayerDamagedEvent e)
    {
        Debug.Log($"🔥 PlayerDamagedEvent: {e.DamageAmount} урону");
    }
    
    private void OnGameOver(GameOverEvent e)
    {
        Debug.Log($"🏆 GameOverEvent: {(e.IsWin ? "ПЕРЕМОГА" : "ПОРАЗКА")}");
        Debug.Log($"Машина рухається: {carController.IsMoving}");
        Debug.Log($"Поточна швидкість: {carController.CurrentSpeed}");
    }
    
    private void OnDestroy()
    {
        eventBus?.Unsubscribe<StartGameEvent>(OnStartGame);
        eventBus?.Unsubscribe<CarReachedEndEvent>(OnCarReachedEnd);
        eventBus?.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        eventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
    }
}