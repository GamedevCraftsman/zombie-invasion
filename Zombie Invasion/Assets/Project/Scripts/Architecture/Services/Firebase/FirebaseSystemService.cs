using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class FirebaseSystemService : IFirebaseSystemService
{
    private readonly FirebaseSettings _firebaseSettings;
    private readonly IEventBus _eventBus;
    private IGoogleSignInService _googleSignInService;
    private IFirebaseAuthService _firebaseAuthService;

    [Inject]
    public FirebaseSystemService(FirebaseSettings firebaseSettings, IEventBus eventBus)
    {
        _firebaseSettings = firebaseSettings;
        _eventBus = eventBus;

        SetUpFirebaseAuthService();
    }

    private void SetUpFirebaseAuthService()
    {
        Debug.Log("Firebase auth service set");
        if (_firebaseSettings.WebClientId == null)
        {
            Debug.LogError("Firebase settings not configured. Add webclient API");
            return;
        }
        
        _googleSignInService = new GoogleSignInService(_firebaseSettings.WebClientId);
        Debug.Log("Google signed in");
         _firebaseAuthService = new FirebaseAuthService();
// #elif UNITY_EDITOR
//         _googleSignInService = new GoogleSignInMock();
//         _firebaseAuthService = new FirebaseAuthMock();

    }

    /*public async void SignInWithGoogle()
    {
        try
        {
            var googleUser = await _googleSignInService.SignInAsync();
            var firebaseUser = await _firebaseAuthService.SignInWithGoogleAsync(googleUser);

            #if ANDROID
            if (firebaseUser != null)
            {
                Debug.Log(
                    $"Firebase user signed in: {firebaseUser.DisplayName} ({firebaseUser.PhoneNumber})");
            }
            _eventBus.Fire(new SignedInEvent());
            #elif UNITY_EDITOR
            _eventBus.Fire(new SignedInEvent());
            Debug.LogWarning($"Firebase user signed in");
            #endif
        }
        catch (Exception e)
        {
            Debug.LogError($"Sign-in failed: {e}");
        }
    }*/
    public async Task SignInWithGoogle()
    {
        try
        {
            var googleUser = await _googleSignInService.SignInAsync();

            if (googleUser != null)
            {
                Debug.Log("Google user: " + googleUser.Email);
            }
            else
            {
                Debug.Log("Google user is null");
            }

            var firebaseUser = await _firebaseAuthService.SignInWithGoogleAsync(googleUser);
            
            if (firebaseUser != null)
            {
                _eventBus.Fire(new SignedInEvent());
                Debug.Log(
                    $"Firebase user signed in: {firebaseUser.DisplayName} ({firebaseUser.Email})");
            }
            else
            {
                Debug.Log("Firebase user is null");
            }
            
// #elif UNITY_EDITOR
//             _eventBus.Fire(new SignedInEvent());
//             Debug.LogWarning("Firebase user signed in (Editor mock).");
        }
        catch (Exception e)
        {
            Debug.LogError($"Sign-in failed: {e}");
            throw;
        }
    }
}