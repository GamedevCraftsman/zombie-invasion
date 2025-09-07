using Zenject;

public class EnemiesCountIncrease
{
    private readonly ProgressSettings _progressSettings;

    [Inject]
    public EnemiesCountIncrease(ProgressSettings progressSettings)
    {
        _progressSettings = progressSettings;
    }

    public int EnemiesIncrease(int enemiesCount)
    {
        enemiesCount += _progressSettings.EnemiesIncrease;
        return enemiesCount;
    }
}