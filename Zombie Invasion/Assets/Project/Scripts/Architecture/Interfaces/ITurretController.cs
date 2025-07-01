using System;

public interface ITurretController
{
    //bool IsControlEnabled { get; }
    float CurrentRotationAngle { get; }

    void EnableControl();
    void DisableControl();
    void SetRotation(float angle);
    void ResetRotation();
    
    //event Action<float> OnRotationChanged;
    public event Action OnGamePlaying;
}