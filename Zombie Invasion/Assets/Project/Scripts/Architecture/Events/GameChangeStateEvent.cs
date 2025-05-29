public class GameStateChangedEvent 
{
    public GameState PreviousState { get; }
    public GameState NewState { get; }
    
    public GameStateChangedEvent(GameState previousState, GameState newState)
    {
        PreviousState = previousState;
        NewState = newState;
    }
}