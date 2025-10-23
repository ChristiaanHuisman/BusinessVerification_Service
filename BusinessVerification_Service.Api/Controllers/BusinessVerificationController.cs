using BusinessVerification_Service.Api.Interfaces.HelpersInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessVerification_Service.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessVerificationController : ControllerBase
    {
        // Inject dependencies
        private readonly IFirebaseHelper _firebaseHelper;

        // Constructor for dependency injection
        public BusinessVerificationController(IFirebaseHelper firebaseHelper)
        {
            _firebaseHelper = firebaseHelper;
        }

        // Standard error message ending for displaying user error messages
        const string errorMessageEnd = "Please ensure all account details are " +
            "correct and try again, contact support if the issue persists.";

        // Bind the Authorization header to the authorizationHeader variable
        [HttpGet("request-business-verification")]
        public async Task<IActionResult> RequestBusinessVerification(
            [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            // Remove tag or set as null
            string? authorizationToken = authorizationHeader?.Replace("Bearer ", "");
            if (string.IsNullOrWhiteSpace(authorizationToken))
            {
                return Unauthorized($"Missing or invalid authorization token. " +
                    $"{errorMessageEnd}");
            }

            // Check token validity
            if (!await _firebaseHelper.VerifyAuthorizationToken(authorizationToken))
            {
                return Unauthorized($"Could not verify authorization token. " +
                    $"{errorMessageEnd}");
            }

            // Return for testing purposes
            return Ok("The request token is validated.");
        }
    }
}
