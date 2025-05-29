public class PlayerDamagedEvent
{
    public int DamageAmount { get; }

    public PlayerDamagedEvent(int damageAmount)
    {
        DamageAmount = damageAmount;
    }
}
