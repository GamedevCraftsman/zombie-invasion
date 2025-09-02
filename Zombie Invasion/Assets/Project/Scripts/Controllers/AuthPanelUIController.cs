using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthPanelUIController : BaseController
{
    [SerializeField] private GameObject authPanel;

    [Header("UI Elements")] 
    [SerializeField] private Button signInButton;

    protected override Task Initialize()
    {
        try
        {
            Subscribe();
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
        signInButton.onClick.AddListener(() =>
        {
 #if UNITY_EDITOR
        SceneManager.LoadScene(1);
        return;
 #endif
            EventBus.Fire(new SignInWithGoogleEvent());
        });
        
    }

    private void Subscribe()
    {
        EventBus.Subscribe<SignedInEvent>(OnSignedIn);
    }

    private void Unsubscribe()
    {
        EventBus?.Unsubscribe<SignedInEvent>(OnSignedIn);
    }

    private void OnSignedIn(SignedInEvent signedInEvent)
    {
        //authPanel.SetActive(false);
        SceneManager.LoadScene(1);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}