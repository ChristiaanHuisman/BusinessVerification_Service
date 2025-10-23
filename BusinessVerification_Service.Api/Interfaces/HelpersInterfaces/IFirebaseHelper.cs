namespace BusinessVerification_Service.Api.Interfaces.HelpersInterfaces
{
    public interface IFirebaseHelper
    {
        Task<bool> VerifyAuthorizationToken(string authorizationToken);
    }
}
