using System.Threading.Tasks;

public class GameManager : BaseManager, IGameManager
{
    public GameState CurrentState { get; }
    protected override Task Initialize()
    {
        throw new System.NotImplementedException();
    }
    
    public void StartGame()
    {
        throw new System.NotImplementedException();
    }

    public void EndGame(bool victory)
    {
        throw new System.NotImplementedException();
    }

    public void RestartGame()
    {
        throw new System.NotImplementedException();
    }
}
