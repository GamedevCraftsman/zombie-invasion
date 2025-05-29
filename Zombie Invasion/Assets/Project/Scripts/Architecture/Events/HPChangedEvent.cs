public class HPChangedEvent 
{
    public int CurrentHP { get; }
    public int MaxHP { get; }
    public float HPPercentage { get; }
    
    public HPChangedEvent(int currentHP, int maxHP)
    {
        CurrentHP = currentHP;
        MaxHP = maxHP;
        HPPercentage = maxHP > 0 ? (float)currentHP / maxHP : 0f;
    }
}