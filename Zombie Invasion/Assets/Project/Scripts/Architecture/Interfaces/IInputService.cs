using UnityEngine;

    public interface IInputService
    {
        Vector2 TouchPosition { get; }
        bool IsTouching { get; }
        bool WasTappedThisFrame { get; }
        event System.Action OnGameStartTap;
    }
