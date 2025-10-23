namespace BusinessVerification_Service.Api.Interfaces.HelpersInterfaces
{
    // Include methods from the helper
    public interface IFirebaseHelper
    {
        Task<bool> VerifyAuthorizationToken(string authorizationToken);

        Task<string?> GetUserIdFromToken(string authorizationToken);
    }
}
