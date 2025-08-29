using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class FirebaseManager: BaseManager
{
    private IFirebaseSystemService _firebaseSystemService;

    [Inject]
    public void Construct(IFirebaseSystemService firebaseSystemService)
    {
        _firebaseSystemService = firebaseSystemService;
    }
    
    protected override Task Initialize()
    {
        try
        {
            Subscribe();
            EventBus.Fire(new SignInWithGoogleEvent());
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
        return Task.CompletedTask;
    }

    private void Subscribe()
    {
        EventBus.Subscribe<SignInWithGoogleEvent>(InGoogleSignIn);
    }

    private void Unsubscribe()
    {
        EventBus?.Unsubscribe<SignInWithGoogleEvent>(InGoogleSignIn);
    }

    private async void InGoogleSignIn(SignInWithGoogleEvent signInWithGoogleEvent)
    {
        try
        {
            await _firebaseSystemService.SignInWithGoogle();
        }
        catch (Exception e)
        {
            Debug.LogError($"Sign-in event failed: {e}");
        }
    }
    
    private void OnDestroy()
    {
        Unsubscribe();
    }
}