using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Google;
using UnityEngine;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth _auth;

    void Start()
    {
        _auth = FirebaseAuth.DefaultInstance;
        SignInWithGoogleButton();
    }

    public void SignInWithGoogleButton()
    {
        SignInWithGoogle();
    }

    private void SignInWithGoogle()
    {
        try
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration {
                RequestIdToken = true,
                // Copy this value from the google-service.json file.
                // oauth_client with type == 3
                WebClientId = "414828740578-fe9u89jpj22305jdl9rd2e5k9gphpatf.apps.googleusercontent.com",
                RequestEmail = true,
                RequestAuthCode = true,    // <-- важливо для refresh/access токенів
                RequestProfile = true,
            };

            Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

            TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser> ();
            signIn.ContinueWith (task => {
                if (task.IsCanceled) {
                    signInCompleted.SetCanceled ();
                } else if (task.IsFaulted) {
                    signInCompleted.SetException (task.Exception);
                } else {

                    Credential credential = GoogleAuthProvider.GetCredential ((task).Result.IdToken, (task).Result.AuthCode);
                    _auth.SignInWithCredentialAsync (credential).ContinueWith (authTask => {
                        if (authTask.IsCanceled) {
                            signInCompleted.SetCanceled();
                        } else if (authTask.IsFaulted) {
                            signInCompleted.SetException(authTask.Exception);
                        } else {
                            signInCompleted.SetResult((authTask).Result);
                        }
                    });
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Google Sign-In failed: {e}");
        }
    }
}