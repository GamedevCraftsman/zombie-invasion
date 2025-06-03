using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : BaseManager, IUIManager
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button continueButton;

    protected override Task Initialize()
    {
        SetButtonsEvents();
        return Task.CompletedTask;
    }

    private void SetButtonsEvents()
    {
        restartButton.onClick.AddListener(() => ShowGameOverUI(false));
        continueButton.onClick.AddListener(() => ShowGameOverUI(true));
    }
    
    public void ShowGameUI()
    {
        throw new System.NotImplementedException();
    }

    public void ShowGameOverUI(bool victory)
    {
        if (victory)
        {
            EventBus.Fire(new ContinueGameEvent());
        }
        else
        {
            Debug.LogWarning("Restart");
            EventBus.Fire(new RestarGameEvent());
        }
    }
}