using System.Threading.Tasks;
using Firebase.Auth;
using Google;

public interface IFirebaseAuthService
{ 
    Task<FirebaseUser> SignInWithGoogleAsync(GoogleSignInUser googleUser);
}