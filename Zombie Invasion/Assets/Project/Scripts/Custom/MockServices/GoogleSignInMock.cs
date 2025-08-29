using System.Threading.Tasks;
using Google;

public class GoogleSignInMock : IGoogleSignInService
{
    public Task<GoogleSignInUser> SignInAsync()
    {
        return Task.FromResult(new GoogleSignInUser
        {
            DisplayName = "Test User",
            Email = "test@example.com",
            IdToken = "dummy-id-token",
            AuthCode = "dummy-auth-code"
        });
    }
}