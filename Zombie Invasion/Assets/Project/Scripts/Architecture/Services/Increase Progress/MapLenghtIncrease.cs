using Zenject;

public class MapLenghtIncrease
{
    private readonly ProgressSettings _progressSettings;
    
    [Inject]
    public MapLenghtIncrease(ProgressSettings progressSettings)
    {
        _progressSettings = progressSettings;
    }

    public void IncreaseMapLenght(ref int currentIncrease)
    {
       currentIncrease += _progressSettings.MapLenghtIncrease;
    }
}