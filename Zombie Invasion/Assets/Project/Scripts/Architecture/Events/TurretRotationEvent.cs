public class TurretRotationEvent
{
    public float CurrentAngle { get; }
    public float MaxAngle { get; }

    public TurretRotationEvent(float currentAngle, float maxAngle)
    {
        CurrentAngle = currentAngle;
        MaxAngle = maxAngle;
    }
}