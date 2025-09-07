public class HPChangedEvent
{
    public int CurrentHp { get; }
    public int MaxHp { get; }
    public float HpPercentage { get; }

    public HPChangedEvent(int currentHp, int maxHp)
    {
        CurrentHp = currentHp;
        MaxHp = maxHp; 
        HpPercentage = maxHp > 0 ? (float)currentHp / maxHp : 0f;
    }
}