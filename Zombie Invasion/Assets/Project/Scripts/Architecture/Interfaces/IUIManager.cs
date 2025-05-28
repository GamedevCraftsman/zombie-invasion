public interface IUIManager
    {
        void ShowGameUI();
        void ShowGameOverUI(bool victory);
        void HideAllUI();
        void UpdateHealthBar(float normalizedHealth);
    }

