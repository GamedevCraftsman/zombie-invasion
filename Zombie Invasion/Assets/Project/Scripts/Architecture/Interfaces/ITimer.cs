public interface ITimer
{
    bool IsOpened { get; }
    void ShowTimerPanel();
    void HideTimerPanel();
}