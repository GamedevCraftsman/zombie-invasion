using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Pool<T> : IPool<T> where T : Component
{
    private readonly List<T> _allItems;
    private readonly IPoolable<T> _poolable;
    private readonly Transform _parentTransform;
    
    public bool IsAvailable => AvailableCount > 0;
    public int AvailableCount => _allItems.Count(item => item != null && !item.gameObject.activeSelf);
    
    public Pool(IPoolable<T> poolable, int initialSize, Transform parentTransform = null)
    {
        _poolable = poolable;
        _parentTransform = parentTransform;
        _allItems = new List<T>(initialSize);
        
        WarmUp(initialSize);
    }
    
    private void WarmUp(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var newItem = _poolable.OnCreate();
            if (_parentTransform != null)
            {
                newItem.transform.SetParent(_parentTransform);
            }
            _poolable.OnRelease(newItem);
            _allItems.Add(newItem);
        }
    }
    
    public T Get()
    {
        var item = _allItems.FirstOrDefault(x => x != null && !x.gameObject.activeSelf);
        
        if (item == null)
        {
            item = _poolable.OnCreate();
            if (_parentTransform != null)
            {
                item.transform.SetParent(_parentTransform);
            }
            _allItems.Add(item);
        }
        
        _poolable.OnGet(item);
        return item;
    }
    
    public void Release(T item)
    {
        if (item == null) return;
        _poolable.OnRelease(item);
    }
    
    public void ReleaseAll()
    {
        foreach (var item in _allItems.Where(x => x != null && x.gameObject.activeSelf))
        {
            _poolable.OnRelease(item);
        }
    }
    
    public void DestroyAll()
    {
        foreach (var item in _allItems.Where(x => x != null))
        {
            _poolable.OnDestroy(item);
            Object.Destroy(item.gameObject);
        }
        _allItems.Clear();
    }
}