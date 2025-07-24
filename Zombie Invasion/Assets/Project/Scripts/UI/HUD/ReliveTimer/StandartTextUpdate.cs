public class StandartTextUpdate : ITextUpdate
{
    public void UpdateText(TMPro.TMP_Text text, string newText)
    {
        text.text = newText;
    }
}