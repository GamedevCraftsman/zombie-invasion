using UnityEngine;

public interface IPool<T> where T : Component
{
    T Get();
    void Release(T item);
    void ReleaseAll();
    bool IsAvailable { get; }
    int AvailableCount { get; }
}