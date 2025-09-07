using UnityEngine;

[CreateAssetMenu(fileName = "TransitionPanelSettings", menuName = "UI/Transition Panel Settings")]
public class TransitionPanelSettings : ScriptableObject
{
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float delayBeforeHideTransitionPanel = 1.5f;

    #region Public values

    public float FadeDuration => fadeDuration;
    public float DelayBeforeHideTransitionPanel => delayBeforeHideTransitionPanel;

    #endregion
}