using System;

public interface ITurretController
{
    bool IsControlEnabled { get; }
    float CurrentRotationAngle { get; }

    void EnableControl();
    void DisableControl();
    void ResetRotation();

    event Action<float> OnRotationChanged;
}