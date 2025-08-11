using Zenject;

public class EnemiesCountIncrease
{
    private readonly ProgressSettings _progressSettings;

    [Inject]
    public EnemiesCountIncrease(ProgressSettings progressSettings)
    {
        _progressSettings = progressSettings;
    }

    public void EnemiesIncrease(ref int enemiesCount)
    {
        enemiesCount += _progressSettings.EnemiesIncrease;
    }
}