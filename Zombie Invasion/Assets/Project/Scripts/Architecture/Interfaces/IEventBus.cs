public interface IEventBus
    {
        void Subscribe<T>(System.Action<T> handler);
        void Unsubscribe<T>(System.Action<T> handler);
        void Fire<T>(T eventData);
        void LogActiveSubscriptions();
        void Clear();
    }
