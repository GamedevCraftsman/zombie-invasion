using System.Threading.Tasks;

public class UIManager : BaseManager, IUIManager
{
    protected override Task Initialize()
    {
        throw new System.NotImplementedException();
    }

    public void ShowGameUI()
    {
        throw new System.NotImplementedException();
    }

    public void ShowGameOverUI(bool victory)
    {
        throw new System.NotImplementedException();
    }

    public void HideAllUI()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateHealthBar(float normalizedHealth)
    {
        throw new System.NotImplementedException();
    }
}
