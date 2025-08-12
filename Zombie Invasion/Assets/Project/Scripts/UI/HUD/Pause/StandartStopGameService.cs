using UnityEngine;

public class StandartStopGameService : IStopGameService
{
    public void Stop()
    {
        Time.timeScale = 0;
    }

    public void Continue()
    {
        Time.timeScale = 1;
    }
}