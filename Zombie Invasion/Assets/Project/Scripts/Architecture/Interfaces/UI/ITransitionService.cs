using System;

public interface ITransitionService
{
    void ChangeEvent(Action eventAction);
}