using Zenject;

public class MapLenghtIncrease
{
    private readonly ProgressSettings _progressSettings;
    
    [Inject]
    public MapLenghtIncrease(ProgressSettings progressSettings)
    {
        _progressSettings = progressSettings;
    }

    public int IncreaseMapLenght(int currentIncrease)
    {
       currentIncrease += _progressSettings.MapLenghtIncrease;
       return currentIncrease;
    }
}