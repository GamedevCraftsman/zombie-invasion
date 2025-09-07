using System;

public interface ITurretController
{
    void EnableControl();
    void DisableControl();
    void SetRotation(float angle);
    void ResetRotation();
    
    public event Action OnGamePlaying;
}