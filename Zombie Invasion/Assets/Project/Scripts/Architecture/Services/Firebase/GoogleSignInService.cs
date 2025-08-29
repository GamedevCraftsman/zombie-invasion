using System.Threading.Tasks;
using Google;

public class GoogleSignInService : IGoogleSignInService
{
    public GoogleSignInService(string webClientId)
    {
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true,
            RequestAuthCode = true,
            RequestProfile = true,
        };
    }

    public Task<GoogleSignInUser> SignInAsync()
    {
        var tcs = new TaskCompletionSource<GoogleSignInUser>();

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
        {
            if (task.IsCanceled) tcs.SetCanceled();
            else if (task.IsFaulted) tcs.SetException(task.Exception);
            else tcs.SetResult(task.Result);
        });

        return tcs.Task;
    }
}