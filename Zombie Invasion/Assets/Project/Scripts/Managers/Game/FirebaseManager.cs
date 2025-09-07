using System;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;
using Zenject;

public class FirebaseManager: BaseManager
{
    private IFirebaseSystemService _firebaseSystemService;
    private DataManageService _dataManageService;

    [Inject]
    public void Construct(IFirebaseSystemService firebaseSystemService, DataManageService dataManageService)
    {
        _firebaseSystemService = firebaseSystemService;
        _dataManageService = dataManageService;
    }
    
    protected override Task Initialize()
    {
        try
        {
            Subscribe();
            DontDestroyOnLoad(this);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
        return Task.CompletedTask;
    }

    private void Start()
    {
        EventBus.Fire(new SignInWithGoogleEvent());
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
            FirebaseUser user = await _firebaseSystemService.SignInWithGoogle();
            if (user != null)
            {
                await _dataManageService.InitialLoad(user);
                EventBus.Fire(new SignedInEvent());
            }
            else 
               EventBus.Fire(new CanceledSignInWithGoogleEvent());
        }
        catch (Exception e)
        {
            Debug.LogError($"Sign-in event failed (firebase manager): {e}");
        }
    }
    
    private void OnDestroy()
    {
        Unsubscribe();
    }
}