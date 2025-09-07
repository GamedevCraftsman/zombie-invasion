public interface ICarHPUIController
{
    void ShowHpBar();
    void HideHpBar();
    void ResetUI();
    void UpdateHp(float hpPercentage, int currentHp, int maxHp);
}