using TMPro;

public class ProgressUIUpdateService : IProgressUIUpdateService
{
    private readonly TMP_Text _lvlText;

    public ProgressUIUpdateService(TMP_Text lvlText)
    {
        _lvlText = lvlText;
    }

    public void ChangeLevelText(string lvlText)
    {
        _lvlText.text = lvlText;
    }
}