using BusinessVerification_Service.Api.Interfaces.HelpersInterfaces;
using FirebaseAdmin.Auth;

namespace BusinessVerification_Service.Api.Helpers
{
    public class FirebaseHelper : IFirebaseHelper
    {
        // Return true if the token is a valid Firebase token
        public async Task<bool> VerifyAuthorizationToken(string authorizationToken)
        {
            try
            {
                // Validate the token signature, issuer and expiry
                // using Firebase public keys
                FirebaseToken decodedToken = await
                    FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(
                    authorizationToken);

                // If the decoding succeeds, the token is valid
                return decodedToken != null;
            }
            catch
            {
                // If the token is not valid
                return false;
            }
        }

        // Return the realted user UID of the validated token
        public async Task<string?> GetUserIdFromToken(string authorizationToken)
        {
            try
            {
                // Validate the token signature, issuer and expiry
                // using Firebase public keys
                FirebaseToken decodedToken = await
                    FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(
                    authorizationToken);

                // Get the user UID from the decoded token
                return decodedToken?.Uid;
            }
            catch
            {
                // If the user UID cannot be returned
                return null;
            }
        }
    }
}
