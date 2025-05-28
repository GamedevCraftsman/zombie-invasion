public interface IGameManager
{
    GameState CurrentState { get; }

    void StartGame();

    //void PauseGame();
    void EndGame(bool victory);
    void RestartGame();
}

public enum GameState
{
    Menu,
    Playing,

    //Paused,
    GameOver,
    Victory
}