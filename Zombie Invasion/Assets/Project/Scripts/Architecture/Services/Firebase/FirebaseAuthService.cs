using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Google;
using UnityEngine;

public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly FirebaseAuth _auth;

    public FirebaseAuthService()
    {
        _auth = FirebaseAuth.DefaultInstance;
    }

    public async Task<FirebaseUser> SignInWithGoogleAsync(GoogleSignInUser googleUser)
    {
        try
        {
            Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, googleUser.AuthCode);

            if (_auth == null)
            {
                Debug.LogWarning("Google Sign-In with Google ID token failed.");
                return null;
            }
            AuthResult result = await _auth.SignInAndRetrieveDataWithCredentialAsync(credential);
            
            return result.User;
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Firebase sign-in error: {ex.Message}");
            return null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Unexpected sign-in error: {ex.Message}");
            return null;
        }
    }
}