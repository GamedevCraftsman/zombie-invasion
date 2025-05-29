/*
using UnityEngine;
using Zenject;

/// <summary>
/// Test script to verify the EventBus functionality
/// Attach to any GameObject in the scene
/// </summary>
public class EventBusTestScript : MonoBehaviour
{
    [Inject] private IEventBus eventBus;
    
    [Header("Test Settings")]
    [SerializeField] private bool autoTestOnStart = true;
    [SerializeField] private float testInterval = 2f;
    
    private float timer = 0f;
    private int testCounter = 0;

    private void Start()
    {
        Debug.Log("=== EventBus Test Started ===");
        
        // Subscribe to all test events
        SubscribeToEvents();
        
        if (autoTestOnStart)
        {
            // Trigger a few initial test events
            RunInitialTests();
        }
    }

    private void Update()
    {
        // Automatic testing every N seconds
        if (autoTestOnStart)
        {
            timer += Time.deltaTime;
            if (timer >= testInterval)
            {
                timer = 0f;
                RunPeriodicTest();
            }
        }
        
        // Test via keyboard input
        HandleKeyboardInput();
    }

    private void SubscribeToEvents()
    {
        Debug.Log("Subscribing to events...");
        
        eventBus.Subscribe<StartGameEvent>(OnGameStarted);
        eventBus.Subscribe<CarReachedEndEvent>(OnGameEnded);
        eventBus.Subscribe<EnemyDestroyedEvent>(OnEnemyDestroyed);
        eventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        
        Debug.Log("Subscription complete!");
    }

    private void RunInitialTests()
    {
        Debug.Log("--- Running initial tests ---");
        
        // Test 1: Simple event without data
        Debug.Log("Test 1: Game Start");
        eventBus.Fire(new StartGameEvent());
        
        // Short delay between tests
        Invoke(nameof(Test2), 0.5f);
    }
    
    private void Test2()
    {
        // Test 2: Event with boolean parameter
        Debug.Log("Test 2: Game End (Defeat)");
        eventBus.Fire(new CarReachedEndEvent ());
        
        Invoke(nameof(Test3), 0.5f);
    }
    
    private void Test3()
    {
        // Test 3: Event with multiple parameters
        Debug.Log("Test 3: Enemy Destroyed");
        eventBus.Fire(new EnemyDestroyedEvent 
        { 
            Position = new Vector3(5, 0, 10),
            EnemiesLeft = 7 
        });
        
        //Invoke(nameof(Test4), 0.5f);
    }
    
    // private void Test4()
    // {
    //     // Test 4: Event with float parameters
    //     Debug.Log("Test 4: Player Damaged");
    //     eventBus.Fire(new PlayerDamagedEvent 
    //     { 
    //         DamageAmount = 65
    //     });
    // }

    private void RunPeriodicTest()
    {
        testCounter++;
        Debug.Log($"Periodic test #{testCounter}");
        
        // Randomized test
        int randomTest = Random.Range(1, 5);
        switch (randomTest)
        {
            case 1:
                eventBus.Fire(new StartGameEvent());
                break;
            case 2:
                eventBus.Fire(new CarReachedEndEvent());
                break;
            case 3:
                eventBus.Fire(new EnemyDestroyedEvent 
                { 
                    Position = Random.insideUnitSphere * 10f,
                    EnemiesLeft = Random.Range(0, 10) 
                });
                break;
            // case 4:
            //     eventBus.Fire(new PlayerDamagedEvent 
            //     { 
            //         CurrentHealth = Random.Range(10f, 100f),
            //         MaxHealth = 100f 
            //     });
            //     break;
        }
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Key 1: GameStarted");
            eventBus.Fire(new StartGameEvent());
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Key 2: GameEnded (Victory)");
            eventBus.Fire(new CarReachedEndEvent());
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Key 3: EnemyDestroyed");
            eventBus.Fire(new EnemyDestroyedEvent 
            { 
                Position = transform.position,
                EnemiesLeft = 5 
            });
        }
        
        // if (Input.GetKeyDown(KeyCode.Alpha4))
        // {
        //     Debug.Log("Key 4: PlayerDamaged");
        //     eventBus.Fire(new PlayerDamagedEvent 
        //     { 
        //         CurrentHealth = 50f,
        //         MaxHealth = 100f 
        //     });
        // }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("EventBus Info:");
            eventBus.LogActiveSubscriptions();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Clearing EventBus...");
            eventBus.Clear();
        }
    }

    // === EVENT HANDLERS ===
    
    private void OnGameStarted(StartGameEvent eventData)
    {
        Debug.Log("RECEIVED: GameStarted - Game has started!");
    }
    
    private void OnGameEnded(CarReachedEndEvent eventData)
    {
        string result = eventData.Victory ? "VICTORY! 🎉" : "DEFEAT 😞";
        Debug.Log($"RECEIVED: GameEnded - {result}");
    }
    
    private void OnEnemyDestroyed(EnemyDestroyedEvent eventData)
    {
        Debug.Log($"RECEIVED: EnemyDestroyed" +
                  $"\n   Position: {eventData.Position}" +
                  $"\n   Remaining: {eventData.EnemiesLeft}");
    }
    
    private void OnPlayerDamaged(PlayerDamagedEvent eventData)
    {
        float healthPercent = (eventData.CurrentHealth / eventData.MaxHealth) * 100f;
        Debug.Log($"RECEIVED: PlayerDamaged" +
                  $"\n   HP: {eventData.CurrentHealth}/{eventData.MaxHealth}" +
                  $"\n   Percent: {healthPercent:F1}%");
    }

    private void OnDestroy()
    {
        // ALWAYS unsubscribe!
        Debug.Log("EventBusTest: Unsubscribing from events...");
        
        if (eventBus != null)
        {
            eventBus.Unsubscribe<StartGameEvent>(OnGameStarted);
            eventBus.Unsubscribe<CarReachedEndEvent>(OnGameEnded);
            eventBus.Unsubscribe<EnemyDestroyedEvent>(OnEnemyDestroyed);
            eventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
            
            Debug.Log("Unsubscription complete!");
        }
    }

    // Buttons in Inspector for manual testing
    [ContextMenu("Test GameStarted")]
    private void TestGameStarted()
    {
        eventBus.Fire(new StartGameEvent());
    }
    
    [ContextMenu("Test GameEnded Victory")]
    private void TestGameEndedVictory()
    {
        eventBus.Fire(new CarReachedEndEvent { Victory = true });
    }
    
    [ContextMenu("Test Enemy Destroyed")]
    private void TestEnemyDestroyed()
    {
        eventBus.Fire(new EnemyDestroyedEvent 
        { 
            Position = Vector3.zero,
            EnemiesLeft = 3 
        });
    }
}
*/
