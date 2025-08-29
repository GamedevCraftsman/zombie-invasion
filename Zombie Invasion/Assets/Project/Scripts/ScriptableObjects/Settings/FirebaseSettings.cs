using UnityEngine;

[CreateAssetMenu(fileName = "FirebaseSettings", menuName = "Firebase/Firebase Settings")]
public class FirebaseSettings : ScriptableObject
{
    [SerializeField] private string webClientId;

    #region Public values

    public string WebClientId => webClientId;

    #endregion
}