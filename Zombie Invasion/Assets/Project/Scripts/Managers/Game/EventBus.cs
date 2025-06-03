using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : IEventBus
{
    private Dictionary<Type, Delegate> _eventHandlers = new Dictionary<Type, Delegate>();

    public void Subscribe<T>(Action<T> handler)
    {
        var eventType = typeof(T);
        if (_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType] = Delegate.Combine(_eventHandlers[eventType], handler);
        }
        else
        {
            _eventHandlers[eventType] = handler;
        }
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        var eventType = typeof(T);
        if (_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType] = Delegate.Remove(_eventHandlers[eventType], handler);

            if (_eventHandlers[eventType] == null)
            {
                _eventHandlers.Remove(eventType);
            }
        }
    }

    public void Fire<T>(T eventData)
    {
        var eventType = typeof(T);
        if (_eventHandlers.TryGetValue(eventType, out var handler))
        {
            var typedHandler = handler as Action<T>;

            try
            {
                typedHandler?.Invoke(eventData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in event handler for {eventType.Name}: {ex.Message}");
                Debug.LogException(ex);
            }
        }
    }

    public void Clear()
    {
        _eventHandlers.Clear();
    }

    public void LogActiveSubscriptions()
    {
        Debug.LogWarning($"EventBus: {_eventHandlers.Count} active event types");
        foreach (var kvp in _eventHandlers)
        {
            var eventType = kvp.Key;
            var handler = kvp.Value;
            var subscriberCount = handler?.GetInvocationList().Length ?? 0;
            Debug.LogWarning($"  {eventType.Name}: {subscriberCount} subscribers");
        }
    }
}