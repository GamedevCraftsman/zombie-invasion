public interface IInputController
{
    InputType LastInputType { get; }
    void ResetForNewGame();
}

public enum InputType
{
    None,
    Keyboard,
    Mouse,
    Touch
}