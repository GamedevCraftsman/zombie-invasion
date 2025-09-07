using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Controllers
{
    public class InputController : BaseController, IInputController
    {
        [Header("Input Settings")] [SerializeField]
        private bool enableMouseInput = true;

        [SerializeField] private bool enableTouchInput = true;

        // State
        private InputType _lastInputType = InputType.None;
        public InputType LastInputType => _lastInputType;

        private Coroutine _checkStartGameCoroutine;
        private IGameManager _gameManager;

        [Inject]
        public void Construct(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        protected override Task Initialize()
        {
            try
            {
                SubscribeToEvents();
                ResetInputState();
                StartToCheckInput();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return Task.CompletedTask;
        }

        #region Events

        private void SubscribeToEvents()
        {
            EventBus.Subscribe<AllowStartGameEvent>(OnAllowStartGame);
        }

        private void UnsubscribeFromEvents()
        {
            EventBus?.Unsubscribe<AllowStartGameEvent>(OnAllowStartGame);
        }

        private void StartGame()
        {
            EventBus.Fire(new ReadyGameEvent());
            StopToCheckInput();
        }

        private void OnAllowStartGame(AllowStartGameEvent allowStartGameEvent)
        {
            ResetInputState();
            StartToCheckInput();
        }

        #endregion

        #region Input Checking

        private void StartToCheckInput()
        {
            _checkStartGameCoroutine = StartCoroutine(CheckStartGame());

            Debug.LogWarning("StartToCheckInput");
        }

        private void StopToCheckInput()
        {
            StopCoroutine(_checkStartGameCoroutine);

            Debug.LogWarning("StopToCheckInput");
        }

        private IEnumerator CheckStartGame()
        {
            while (true)
            {
                InputType detectedInput = DetectInput();

                if (detectedInput != InputType.None && _gameManager.CurrentState == GameState.Menu)
                {
                    _lastInputType = detectedInput;
                    StartGame();
                    yield break;
                }

                yield return null;
            }
        }

        #endregion

        #region Input Detection

        private InputType DetectInput()
        {
            InputType detectedInput = InputType.None;

            CheckMouseInput(ref detectedInput);
            CheckMobileInput(ref detectedInput);

            return detectedInput;
        }

        private void CheckMouseInput(ref InputType detectInputType)
        {
            if (enableMouseInput && Input.GetMouseButtonDown(0))
            {
                detectInputType = InputType.Mouse;
            }
        }

        private void CheckMobileInput(ref InputType detectInputType)
        {
            if (enableTouchInput && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    detectInputType = InputType.Touch;
                }
            }
        }

        #endregion

        public void ResetForNewGame()
        {
            ResetInputState();
        }

        private void ResetInputState()
        {
            _lastInputType = InputType.None;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}