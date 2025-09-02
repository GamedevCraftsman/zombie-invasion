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
    }
    
    public async Task SignInWithGoogle()
    {
        try
        {
            var googleUser = await _googleSignInService.SignInAsync();
            var firebaseUser = await _firebaseAuthService.SignInWithGoogleAsync(googleUser);
            
            if (firebaseUser != null)
            {
                _eventBus.Fire(new SignedInEvent());
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Sign-in failed: {e}");
            throw;
        }
    }
}