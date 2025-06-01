using System;
using UnityEngine;

public interface IInputController
{
    // Properties
    bool IsInputEnabled { get; }
    bool IsGameStarted { get; }
    InputType LastInputType { get; }
    
    // Input control methods
    void EnableInput();
    void DisableInput();
    void ResetForNewGame();
    
    // Input configuration
    //void SetStartGameKey(KeyCode newKey);
    void EnableKeyboardInput(bool enable);
    void EnableMouseInput(bool enable);
    void EnableTouchInput(bool enable);
    
    // Events
    //event Action OnGameStartRequested;
    //event Action<InputType> OnInputDetected;
}

public enum InputType
{
    None,
    Keyboard,
    Mouse,
    Touch
}