using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
[CreateAssetMenu(fileName = "UISettings", menuName = "UI/UI Settings")]
public class UISettings : ScriptableObject
{
    [Header("Panels")] 
    [SerializeField] private float disappearPanelTime = 1;
    [SerializeField] private float appearPanelTime = 2;

    [Header("Button")] 
    [SerializeField] private float buttonMoveTime = 2;
    [SerializeField] private Ease buttonMoveEase = Ease.OutBack;
    [SerializeField] private float buttonStartPos = -200;
    [SerializeField] private float buttonEndPos = 190;

    [Header("Relive Panel")] 
    [SerializeField] private int timeToRelive = 20;
    
    [Header("Pause")]
    [SerializeField] private float showPauseButtonTime = 0.5f;
    [SerializeField] private Ease showPauseButtonEase = Ease.OutQuad;
    [SerializeField] private float showContinueButtonTime = 0.5f;
    [SerializeField] private float showMainMenuButtonTime = 0.5f;
    [SerializeField] private Ease showPauseButtonsEase;
    
    [Header("Main Screen")]
    [Header("   - Progress UI")]
    [SerializeField] private float showHideProgressUITime = 0.2f;
    [FormerlySerializedAs("positionOffset")]
    [Header("   - TapToPlay UI")]
    [SerializeField] private float tapToPlayPositionOffset = 50f;
    [SerializeField] private float timeToMoveTapToPlay = 1f;
    [SerializeField] private LoopType tapToPlayLoop = LoopType.Yoyo;

    #region Public values

    public float DisappearPanelTime => disappearPanelTime;

    public float AppearPanelTime => appearPanelTime;

    //Button Settings
    public Ease ButtonMoveEase => buttonMoveEase;
    public float ButtonMoveTime => buttonMoveTime;
    public float ButtonStartPos => buttonStartPos;
    public float ButtonEndPos => buttonEndPos;

    //Relive Panel
    public int TimeToRelive => timeToRelive;
    
    //Pause
    public float ShowPauseButtonTime => showPauseButtonTime;
    public float ShowContinueButtonTime => showContinueButtonTime;
    public float ShowMainMenuButtonTime => showMainMenuButtonTime;
    public Ease ShowPauseButtonEase => showPauseButtonEase;
    public Ease ShowPauseButtonsEase => showPauseButtonsEase;
    
    //Main menu
    public float TapToPlayPositionOffset => tapToPlayPositionOffset;
    public float TimeToMoveTapToPlay => timeToMoveTapToPlay;
    public LoopType TapToPlayLoop => tapToPlayLoop;
    public float ShowHideProgressUITime => showHideProgressUITime;
    #endregion
}