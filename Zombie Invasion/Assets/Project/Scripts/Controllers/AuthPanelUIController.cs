using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthPanelUIController : BaseController
{
    [SerializeField] private GameObject authPanel;

    [Header("UI Elements")] 
    [SerializeField] private Button signInButton;
    [SerializeField] private TMP_Text signInButtonText;

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
        signInButton.onClick.AddListener(() => EventBus.Fire(new SignInWithGoogleEvent()));
        
    }

    private void Subscribe()
    {
        EventBus.Subscribe<SignedInEvent>(OnSignedIn);
        EventBus.Subscribe<SignInWithGoogleEvent>(OnSighInWithGoogle);
        EventBus.Subscribe<CanceledSignInWithGoogleEvent>(OnCancelSignInWithGoogle);
    }

    private void Unsubscribe()
    {
        EventBus?.Unsubscribe<SignedInEvent>(OnSignedIn);
        EventBus?.Unsubscribe<SignInWithGoogleEvent>(OnSighInWithGoogle);
        EventBus?.Unsubscribe<CanceledSignInWithGoogleEvent>(OnCancelSignInWithGoogle);
    }

    private void OnSignedIn(SignedInEvent signedInEvent)
    {
        SceneManager.LoadScene(1);
    }

    private void OnSighInWithGoogle(SignInWithGoogleEvent signInWithGoogleEvent)
    {
        signInButton.interactable = false; 
        signInButtonText.SetText("Loading...");
    }

    private void OnCancelSignInWithGoogle(CanceledSignInWithGoogleEvent signInWithGoogleEvent)
    {
        signInButton.interactable = true;
        signInButtonText.SetText("Continue with Google");
    }
    
    private void OnDestroy()
    {
        Unsubscribe();
    }
}