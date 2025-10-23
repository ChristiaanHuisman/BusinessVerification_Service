using BusinessVerification_Service.Api.Dtos;
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
        const string errorMessageEnd = "Please ensure all account details are correct " +
            "and try again in a few minutes, contact support if the issue persists.";

        // Bind the Authorization header to the authorizationHeader variable
        [HttpGet("request-business-verification")]
        public async Task<IActionResult> RequestBusinessVerification(
            [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            // Create response DTO instance
            BusinessVerificationResponseDto responseDto = new();

            // Remove tag or set as null
            string? authorizationToken = authorizationHeader?.Replace("Bearer ", "");
            if (string.IsNullOrWhiteSpace(authorizationToken))
            {
                responseDto.Message = $"Missing or invalid authorization token. " +
                    $"{errorMessageEnd}";
                return Unauthorized(responseDto);
            }

            // (call service here with dto and token)

            // Check token validity
            if (!await _firebaseHelper.VerifyAuthorizationToken(authorizationToken))
            {
                responseDto.Message = $"Could not verify authorization token. " +
                    $"{errorMessageEnd}";
                return Unauthorized(responseDto);
            }

            // Get the relevant user ID
            string? userId = await _firebaseHelper.GetUserIdFromToken(authorizationToken);
            if (userId == null)
            {
                responseDto.Message = $"Could not find necessary user information. " +
                    $"{errorMessageEnd}";
                return NotFound(responseDto);
            }

            // (for testing)
            return Ok("Successfully running API.");

            // (need to move some of this logic to the service rather)
        }
    }
}
