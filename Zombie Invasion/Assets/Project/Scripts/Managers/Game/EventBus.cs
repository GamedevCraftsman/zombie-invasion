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

            // Якщо після видалення не залишилося обробників, видаляємо ключ
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
            // Приводимо Delegate до потрібного типу Action<T>
            var typedHandler = handler as Action<T>;

            try
            {
                // Викликаємо всі підписані обробники
                typedHandler?.Invoke(eventData);
            }
            catch (Exception ex)
            {
                // Логуємо помилку, щоб один зламаний обробник не зупинив інші
                Debug.LogError($"Error in event handler for {eventType.Name}: {ex.Message}");
                Debug.LogException(ex);
            }
        }
    }


    /// Очищує всі підписки. Корисно при зміні сцен або перезапуску гри
    public void Clear()
    {
        _eventHandlers.Clear();
    }
    
    // /// Перевіряє, чи є підписники на конкретний тип події
    // public bool HasSubscribers<T>() where T : struct
    // {
    //     var eventType = typeof(T);
    //     return _eventHandlers.ContainsKey(eventType) && _eventHandlers[eventType] != null;
    // }
    
    /*/// Повертає кількість підписників для конкретного типу події
    public int GetSubscriberCount<T>() where T : struct
    {
        var eventType = typeof(T);
        if (_eventHandlers.TryGetValue(eventType, out var handler) && handler != null)
        {
            // Delegate.GetInvocationList() повертає масив всіх методів у multicast delegate
            return handler.GetInvocationList().Length;
        }

        return 0;
    }*/
    
    /// Debug метод для виведення інформації про всі активні підписки
    public void LogActiveSubscriptions()
    {
        Debug.Log($"EventBus: {_eventHandlers.Count} active event types");
        foreach (var kvp in _eventHandlers)
        {
            var eventType = kvp.Key;
            var handler = kvp.Value;
            var subscriberCount = handler?.GetInvocationList().Length ?? 0;
            Debug.Log($"  {eventType.Name}: {subscriberCount} subscribers");
        }
    }
}