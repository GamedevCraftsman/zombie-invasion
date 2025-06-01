public interface IGameManager
{
    GameState CurrentState { get; }
    void StartGame();
    void EndGame(bool victory);
    void RestartGame();
}

public enum GameState
{
    Menu,
    Ready,
    Playing,
    GameOver,
    Victory
}