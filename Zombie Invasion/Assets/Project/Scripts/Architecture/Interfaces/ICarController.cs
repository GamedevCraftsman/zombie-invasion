public interface ICarController
{
    void ResetCarState();
    void StartMovement();
    void StopMovement();
    void ResetPosition();
    
    void SmoothStop();
    //Delete in future
    //void LvlLenghtCalculation();

}