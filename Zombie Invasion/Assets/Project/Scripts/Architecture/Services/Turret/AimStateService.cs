using UnityEngine;

public class AimStateService : MonoBehaviour, IAimStateService
{
    [SerializeField] private GameObject aim;

    public void AimManage(bool isOn)
    {
        aim.SetActive(isOn);
    }
}