using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthPanelUIController : BaseController
{
    [SerializeField] private GameObject authPanel;

    [Header("UI Elements")] [SerializeField]
    private Button signInButton;

    [SerializeField] private TMP_Text signInButtonText;

    protected override Task Initialize()
    {
        try
        {
            AddListeners();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return Task.CompletedTask;
    }

    private void AddListeners()
    {
        signInButton.onClick.AddListener(() => SceneManager.LoadScene(1));
    }

}