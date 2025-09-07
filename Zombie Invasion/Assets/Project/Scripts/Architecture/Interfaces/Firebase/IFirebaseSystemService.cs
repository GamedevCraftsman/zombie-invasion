using System.Threading.Tasks;
using Firebase.Auth;

public interface IFirebaseSystemService
{
    Task<FirebaseUser> SignInWithGoogle();
}