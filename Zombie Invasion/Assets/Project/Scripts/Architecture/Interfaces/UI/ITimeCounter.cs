using TMPro;
using UnityEngine.UI;

public interface ITimeCounter
{
    public bool CountDownTick(TMP_Text text, Image bar);
    public void ResetCounter(TMP_Text text, Image bar);
}