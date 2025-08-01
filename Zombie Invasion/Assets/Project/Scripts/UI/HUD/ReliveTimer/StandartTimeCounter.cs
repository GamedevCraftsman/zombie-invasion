using TMPro;
using UnityEngine.UI;

public class StandartTimeCounter : ITimeCounter
{
    private readonly ITextUpdate _textUpdater = new StandartTextUpdate();
    private readonly IBarUpdate _barUpdater = new StandartBarUpdate();

    private readonly int _timeToCountDown;
    private int _currentTime;

    public StandartTimeCounter(int timeToCountDown)
    {
        _timeToCountDown = timeToCountDown;
        
        _currentTime = timeToCountDown;
    }
    
    public bool CountDownTick( TMP_Text text, Image bar)
    {
        _currentTime--;

        _textUpdater.UpdateText(text, _currentTime.ToString());
        _barUpdater.UpdateBar(bar, _currentTime, _timeToCountDown);
        
        if (_currentTime <= 0)
        {
            return false;
        }

        return true;
    }

    public void ResetCounter(TMP_Text text, Image bar)
    {
        _textUpdater.UpdateText(text, _timeToCountDown.ToString());
        _barUpdater.ResetBar(bar);
        
        _currentTime = _timeToCountDown;
    }
}