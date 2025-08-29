using System.Threading.Tasks;
using Google;
using JetBrains.Annotations;

public interface IGoogleSignInService
{
    public Task<GoogleSignInUser> SignInAsync();
}