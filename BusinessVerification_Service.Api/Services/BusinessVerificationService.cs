using BusinessVerification_Service.Api.Dtos;
using BusinessVerification_Service.Api.Interfaces.HelpersInterfaces;
using BusinessVerification_Service.Api.Interfaces.ServicesInterfaces;
using Nager.PublicSuffix;
using System.Net.Mail;

namespace BusinessVerification_Service.Api.Services
{
    public class BusinessVerificationService : IBusinessVerificationService
    {
        // Inject dependencies
        private readonly IDomainParser _domainParser;
        private readonly IFirebaseHelper _firebaseHelper;

        // Constructor for dependency injection
        public BusinessVerificationService(IDomainParser domainParser,
            IFirebaseHelper firebaseHelper)
        {
            _domainParser = domainParser;
            _firebaseHelper = firebaseHelper;
        }

        // Standard error message ending for displaying user error messages
        const string errorMessageEnd = $"Please ensure all account details are correct " +
            "and try again in a few minutes, contact support if the issue persists.";

        // Return a response DTO to send back to the user Flutter app
        public async Task<BusinessVerificationResponseDto> BusinessVerificationProcess(
            string? authorizationHeader)
        {
            // Create response DTO instance
            BusinessVerificationResponseDto responseDto = new();

            try
            {
                // Remove tag or set as null
                string? authorizationToken = authorizationHeader?.Replace("Bearer ", "");
                if (string.IsNullOrWhiteSpace(authorizationToken))
                {
                    responseDto.Message = $"Missing or invalid authorization token. " +
                        $"{errorMessageEnd}";
                    return responseDto;
                }

                // Check token validity
                if (!await _firebaseHelper.VerifyAuthorizationToken(authorizationToken))
                {
                    responseDto.Message = $"Could not verify authorization token. " +
                        $"{errorMessageEnd}";
                    return responseDto;
                }

                // Get the relevant user ID
                string? userId = await _firebaseHelper.GetUserIdFromToken(authorizationToken);
                if (userId == null)
                {
                    responseDto.Message = $"Could not find necessary user information. " +
                        $"{errorMessageEnd}";
                    return responseDto;
                }

                // Assign appropriate message to the response DTO based on
                // the verification status
                switch (responseDto.VerificationStatus)
                {
                    case Models.UserVerificationStatus.Accepted:
                        responseDto.Message = $"Your business verification request has been " +
                            $"approved.";
                    break;
                    case Models.UserVerificationStatus.PendingEmail:
                        responseDto.Message = $"Your business verification request is pending " +
                            $"email confirmation. Please check your inbox periodically for " +
                            $"instructions.";
                    break;
                    case Models.UserVerificationStatus.PendingAdmin:
                        responseDto.Message = $"Your verification request is pending review " +
                            $"by an admin. You will be notified once it's processed.";
                    break;
                    case Models.UserVerificationStatus.Rejected:
                        responseDto.Message = $"Your verification request was rejected due " +
                            $"to your business name and domains not matching properly. " +
                            $"{errorMessageEnd}";
                    break;
                    default:
                        responseDto.Message = $"An unexpected error occured during your " +
                            $"business verification request process. {errorMessageEnd}";
                    break;
                }
            }
            catch
            {
                // Handle unexpected errors gracefully
                responseDto.Message = $"An unexpected error occured during your " +
                    $"business verification request process. {errorMessageEnd}";
            }

            return responseDto;
        }

        // Return an array of parsed domain DTOs of email and website
        public ParsedDomainDto[] GetDomainInfo(string emailAddress, string websiteAddress)
        {
            // Get the domains from email and website
            string emailHost = new MailAddress(emailAddress).Host;
            string websiteHost = new Uri(websiteAddress).Host;

            // Get the parsed domain info for email and website
            DomainInfo emailDomainInfo = _domainParser.Parse(emailHost);
            DomainInfo websiteDomainInfo = _domainParser.Parse(websiteHost);

            // Build the DTOs for email and website
            return [
                new ParsedDomainDto
                {
                    RegistrableDomain = emailDomainInfo.RegistrableDomain,
                    TopLevelDomain = emailDomainInfo.TopLevelDomain,
                    Domain = emailDomainInfo.Domain
                },
                new ParsedDomainDto
                {
                    RegistrableDomain = websiteDomainInfo.RegistrableDomain,
                    TopLevelDomain = websiteDomainInfo.TopLevelDomain,
                    Domain = websiteDomainInfo.Domain
                }
            ];
        }
    }
}
