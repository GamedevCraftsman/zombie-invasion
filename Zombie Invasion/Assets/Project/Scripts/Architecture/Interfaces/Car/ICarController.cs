using UnityEngine;

public interface ICarController
{
    GameObject Car { get; }

    void ResetCarState();
    void StartMovement();
    void StopMovement();
    void ResetPosition();
}