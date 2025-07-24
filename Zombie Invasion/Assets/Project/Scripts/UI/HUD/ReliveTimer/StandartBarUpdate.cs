using UnityEngine.UI;

public class StandartBarUpdate : IBarUpdate
{
    private const int BaseBarValue = 1;
    
    public void UpdateBar(Image bar, int value, int max)
    {
        float fill = (float)value / max;
        bar.fillAmount = fill;
    }

    public void ResetBar(Image bar)
    {
        bar.fillAmount = BaseBarValue;
    }
}