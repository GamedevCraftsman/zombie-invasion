using UnityEngine.UI;

public interface IBarUpdate
{
    void UpdateBar(Image bar, int value, int max);
    void ResetBar(Image bar);
}