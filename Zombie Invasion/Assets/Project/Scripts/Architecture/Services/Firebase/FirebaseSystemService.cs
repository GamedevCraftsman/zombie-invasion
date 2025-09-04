using System;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;
using Zenject;

public class FirebaseSystemService : IFirebaseSystemService
{
    private readonly FirebaseSettings _firebaseSettings;
    private IGoogleSignInService _googleSignInService;
    private IFirebaseAuthService _firebaseAuthService;

    [Inject]
    public FirebaseSystemService(FirebaseSettings firebaseSettings)
    {
        _firebaseSettings = firebaseSettings;

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
    }
    
    public async Task<FirebaseUser> SignInWithGoogle()
    {
        try
        {
            var googleUser = await _googleSignInService.SignInAsync();
            if (googleUser == null) return null;
            
            var firebaseUser = await _firebaseAuthService.SignInWithGoogleAsync(googleUser);
            return firebaseUser;
        }
        catch (Exception e)
        {
            Debug.LogError($"Sign-in failed (firebase system service): {e}");
            return null;
        }
    }
}