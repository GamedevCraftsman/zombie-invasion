using UnityEngine;

[CreateAssetMenu(fileName = "GameplayUISettings", menuName = "Game/GameplayUISettings")]
public class GameplayUISettings : ScriptableObject
{
    [Header("Car HP settings")] 
    [SerializeField, Range(0,1)] private float targetFillAmount = 1;
    [SerializeField, Range(0, 1)] private float hpBarEndFadeLvl = 1;
    [SerializeField] private float hpBarShowDuration = 2;

    #region Public Values

    public float TargetFillAmount => targetFillAmount;
    public float HpBarEndFadeLvl => hpBarEndFadeLvl;
    public float HpBarShowDuration => hpBarShowDuration;

    #endregion
}