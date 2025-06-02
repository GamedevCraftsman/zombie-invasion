using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class InputController : BaseController, IInputController
{
    [Inject] private IGameManager _gameManager;

    [Header("Input Settings")] [SerializeField]
    private bool enableKeyboardInput = true;

    [SerializeField] private bool enableMouseInput = true;
    [SerializeField] private bool enableTouchInput = true;
    [SerializeField] private KeyCode startGameKey = KeyCode.Space;

    // State
    private InputType lastInputType = InputType.None;

    public InputType LastInputType => lastInputType;

    protected override Task Initialize()
    {
        try
        {
            SubscribeToEvents();
            ResetInputState();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }

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
        lastInputType = InputType.None;
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        ResetInputState();
    }

    private void Update()
    {
        InputType detectedInput = DetectInput();

        if (detectedInput != InputType.None && _gameManager.CurrentState == GameState.Menu)
        {
            lastInputType = detectedInput;
            StartGame();
        }
    }

    private InputType DetectInput()
    {
        // Check Keyboard input
        if (enableKeyboardInput && Input.GetKeyDown(startGameKey))
        {
            return InputType.Keyboard;
        }

        // Check Mouse input
        if (enableMouseInput && Input.GetMouseButtonDown(0))
        {
            return InputType.Mouse;
        }

        // Check Mobile input (Tap)
        if (enableTouchInput && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                return InputType.Touch;
            }
        }

        return InputType.None;
    }

    private void StartGame()
    {
        EventBus.Fire(new ReadyGameEvent());
    }

    public void ResetForNewGame()
    {
        ResetInputState();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}