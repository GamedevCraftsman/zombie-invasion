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

    
    /*public async void SignInWithGoogleAsync(GoogleSignInUser googleUser)
    {
        Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, googleUser.AuthCode);

        await _auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
        
    }*/
}

// var tcs = new TaskCompletionSource<FirebaseUser>();
// var credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, googleUser.AuthCode);
//
// _auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
// {
//     if (task.IsCanceled) tcs.SetCanceled();
//     else if (task.IsFaulted) tcs.SetException(task.Exception);
//     else tcs.SetResult(task.Result);
// });