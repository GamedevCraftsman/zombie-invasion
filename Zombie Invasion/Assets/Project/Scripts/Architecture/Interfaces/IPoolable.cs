using UnityEngine;

public interface IPoolable<T> where T : Component
{
    T OnCreate();
    void OnGet(T item);
    void OnRelease(T item);
    void OnDestroy(T item);
}