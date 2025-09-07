using System.Threading.Tasks;
using Firebase.Auth;
using Google;

public class FirebaseAuthMock : IFirebaseAuthService
{
    public async Task<FirebaseUser> SignInWithGoogleAsync(GoogleSignInUser googleUser)
    {
        return await Task.FromResult<FirebaseUser>(null);
    }
}