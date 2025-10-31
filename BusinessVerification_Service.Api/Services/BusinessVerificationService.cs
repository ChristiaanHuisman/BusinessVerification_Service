using BusinessVerification_Service.Api.Dtos;
using BusinessVerification_Service.Api.Interfaces.HelpersInterfaces;
using BusinessVerification_Service.Api.Interfaces.ServicesInterfaces;
using BusinessVerification_Service.Api.Models;
using Nager.PublicSuffix;
using System.Net.Mail;

namespace BusinessVerification_Service.Api.Services
{
    public class BusinessVerificationService : IBusinessVerificationService
    {
        // Inject dependencies
        private readonly IDomainParser _domainParser;
        private readonly IFirebaseHelper _firebaseHelper;
        private readonly IDomainNameHelper _domainNameHelper;
        private readonly IWebsiteAddressHelper _websiteAddressHelper;
        private readonly IFirestoreService _firestoreService;
        private readonly INormalizationAndValidationHelper
            _normalizationAndValidationHelper;

        // Constructor for dependency injection
        public BusinessVerificationService(IDomainParser domainParser,
            IFirebaseHelper firebaseHelper, IDomainNameHelper domainNameHelper,
            IWebsiteAddressHelper websiteAddressHelper, IFirestoreService firestoreService,
            INormalizationAndValidationHelper normalizationAndValidationHelper)
        {
            _domainParser = domainParser;
            _firebaseHelper = firebaseHelper;
            _domainNameHelper = domainNameHelper;
            _websiteAddressHelper = websiteAddressHelper;
            _firestoreService = firestoreService;
            _normalizationAndValidationHelper = normalizationAndValidationHelper;
        }

        // Standard error message ending for displaying user error messages
        const string errorMessageEnd = "Please ensure all account details are correct " +
            "and try again in a few minutes, contact support if the issue persists.";

        // Collection names in Firestore
        const string userCollection = "users";
        const string verificationCollection = "businessVerification";

        // Return an tuple of parsed domain DTOs of email and website
        public (ParsedDomainDto ParsedEmailDomain, ParsedDomainDto ParsedWebsiteDomain)
            GetDomainInfo(string emailAddress, string websiteAddress)
        {
            // Get the domains from email and website
            string emailHost = new MailAddress(emailAddress).Host;
            string websiteHost = new Uri(websiteAddress).Host;

            // Get the parsed domain info for email and website
            DomainInfo emailDomainInfo = _domainParser.Parse(emailHost);
            DomainInfo websiteDomainInfo = _domainParser.Parse(websiteHost);

            // Build the DTOs for email and website
            return (
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
            );
        }

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
                    // Returning a response
                    responseDto.Message = $"Missing or invalid authorization token. " +
                        $"{errorMessageEnd}";
                    return responseDto;
                }

                // Check token validity
                if (!await _firebaseHelper.VerifyAuthorizationToken(authorizationToken))
                {
                    // Returning a response
                    responseDto.Message = $"Could not verify authorization token. " +
                        $"{errorMessageEnd}";
                    return responseDto;
                }

                // Get the relevant user ID
                string? userId = await _firebaseHelper.GetUserIdFromToken(authorizationToken);
                if (userId == null)
                {
                    // Returning a response
                    responseDto.Message = $"Could not find user in database. " +
                        $"{errorMessageEnd}";
                    return responseDto;
                }

                // Get relevant Firebase document paths
                string firestoreUserDocumentPath = $"{userCollection}/{userId}";
                string firestoreBusinessVerificationDocumentPath = $"{userCollection}/" +
                    $"{userId}/{verificationCollection}/{userId}";

                // Retrieve documents from Firestore and convert to relevant models
                UserModel? userModel = await _firestoreService.GetDocumentFromFirestore<UserModel>(
                    firestoreUserDocumentPath);
                if (userModel == null)
                {
                    // Returning a response
                    responseDto.Message = $"Could not find user in database. " +
                        $"{errorMessageEnd}";
                    return responseDto;
                }
                else if (userModel.Role != UserRole.Business)
                {
                    // Returning a response
                    responseDto.Message = $"Only business accounts can request business " +
                        $"verification. {errorMessageEnd}";
                    return responseDto;
                }
                BusinessVerificationModel? businessVerificationModel = await
                    _firestoreService.GetDocumentFromFirestore<BusinessVerificationModel>(
                    firestoreBusinessVerificationDocumentPath);
                businessVerificationModel ??= new();
                businessVerificationModel.SetVerificationRequestedAt(userModel);
                businessVerificationModel.SetEmailVerified(userModel);

                // Normalize data
                userModel.Email = _normalizationAndValidationHelper.NormalizeString(
                    userModel.Email);
                userModel.Email = _normalizationAndValidationHelper.RemoveAllWhitespace(
                    userModel.Email);
                userModel.Website = _normalizationAndValidationHelper.NormalizeString(
                    userModel.Website);
                userModel.Website = _normalizationAndValidationHelper.RemoveAllWhitespace(
                    userModel.Website);
                string? businessName = _normalizationAndValidationHelper.NormalizeString(
                    userModel.Name);

                // Validate data exists
                if (!_normalizationAndValidationHelper.IsPopulated(
                    userModel.Email, userModel.Website, businessName))
                {
                    // Execute writing to Firestore documents and returning a response
                    businessVerificationModel.ErrorOccured = true;
                    responseDto.Message = $"Some user data is missing. {errorMessageEnd}";
                    await _firestoreService.SetDocumentByFirestorePath(
                        firestoreUserDocumentPath, userModel);
                    await _firestoreService.SetDocumentByFirestorePath(
                        firestoreBusinessVerificationDocumentPath, businessVerificationModel);
                    return responseDto;
                }

                // Validate email and website format
                if (!_normalizationAndValidationHelper.IsValidEmailAddress(userModel.Email))
                {
                    // Execute writing to Firestore documents and returning a response
                    businessVerificationModel.ErrorOccured = true;
                    responseDto.Message = $"Invalid email address received. {errorMessageEnd}";
                    await _firestoreService.SetDocumentByFirestorePath(
                        firestoreUserDocumentPath, userModel);
                    await _firestoreService.SetDocumentByFirestorePath(
                        firestoreBusinessVerificationDocumentPath, businessVerificationModel);
                    return responseDto;
                }
                if (!_websiteAddressHelper.VerifyWebsiteAddressScheme(userModel.Website))
                {
                    // Execute writing to Firestore documents and returning a response
                    businessVerificationModel.ErrorOccured = true;
                    responseDto.Message = $"Invalid website address scheme received. " +
                        $"{errorMessageEnd}";
                    await _firestoreService.SetDocumentByFirestorePath(
                        firestoreUserDocumentPath, userModel);
                    await _firestoreService.SetDocumentByFirestorePath(
                        firestoreBusinessVerificationDocumentPath, businessVerificationModel);
                    return responseDto;
                }
                userModel.Website = _websiteAddressHelper.BuildUriWebsiteAddress(
                    userModel.Website);

                // Get tuple of parsed domain DTOs for email and website addresses
                (ParsedDomainDto parsedEmailDomain, ParsedDomainDto parsedWebsiteDomain) =
                    GetDomainInfo(userModel.Email, userModel.Website);

                // Handle errors in domain parsing for email and website or invalid formats
                if (parsedEmailDomain == null
                    || string.IsNullOrWhiteSpace(parsedEmailDomain.RegistrableDomain)
                    || parsedEmailDomain.RegistrableDomain == parsedEmailDomain.TopLevelDomain
                    || parsedWebsiteDomain == null
                    || string.IsNullOrWhiteSpace(parsedWebsiteDomain.RegistrableDomain)
                    || parsedWebsiteDomain.RegistrableDomain == parsedWebsiteDomain.TopLevelDomain)
                {
                    // Execute writing to Firestore documents and returning a response
                    businessVerificationModel.ErrorOccured = true;
                    responseDto.Message = $"Website address could not be processed properly and " +
                        $"might have an invalid format. {errorMessageEnd}";
                    await _firestoreService.SetDocumentByFirestorePath(
                        firestoreUserDocumentPath, userModel);
                    await _firestoreService.SetDocumentByFirestorePath(
                        firestoreBusinessVerificationDocumentPath, businessVerificationModel);
                    return responseDto;
                }

                // Use parsed domain information to check if email and website domains match
                if (parsedEmailDomain.RegistrableDomain != parsedWebsiteDomain.RegistrableDomain)
                {
                    // Execute writing to Firestore documents and returning a response
                    userModel.VerificationStatus = UserVerificationStatus.Rejected;
                    businessVerificationModel.SetVerificationStatus(userModel);
                    responseDto.Message = $"Email and website domains do not match. " +
                        $"{errorMessageEnd}";
                    await _firestoreService.SetDocumentByFirestorePath(
                        firestoreUserDocumentPath, userModel);
                    await _firestoreService.SetDocumentByFirestorePath(
                        firestoreBusinessVerificationDocumentPath, businessVerificationModel);
                    return responseDto;
                }

                // Classify model verification status based on fuzzy score
                int fuzzyScore = _domainNameHelper.FuzzyMatchScore(parsedEmailDomain.Domain,
                    businessName);
                businessVerificationModel.FuzzyScore = fuzzyScore;
                switch (fuzzyScore)
                {
                    // For a score of >= 95 the business name can be automatically verified
                    case >= 95:
                        if (userModel.EmailVerified == true)
                        {
                            userModel.VerificationStatus = UserVerificationStatus.Accepted;
                            businessVerificationModel.SetVerificationStatus(userModel);
                        }
                        else
                        {
                            // Call method to trigger the email verification process
                            //
                            // This process is not implemented yet, as using a free domain to
                            // send transactional emails reliably needs a bit of a
                            // workaround, but it does seem possible in ASP.NET Core

                            userModel.VerificationStatus = UserVerificationStatus.PendingEmail;
                            businessVerificationModel.SetVerificationStatus(userModel);
                        }
                    break;

                    // For a score of >= 65 and <= 94 an admin needs to verify the business name
                    case >= 65:
                        userModel.VerificationStatus = UserVerificationStatus.PendingAdmin;
                        businessVerificationModel.SetVerificationStatus(userModel);
                    break;

                    // For a score of <= 64 the business name cannot be verified
                    default:
                        userModel.VerificationStatus = UserVerificationStatus.Rejected;
                        businessVerificationModel.SetVerificationStatus(userModel);
                    break;
                }

                // Assign appropriate message to the response DTO based on
                // the current model verification status
                switch (userModel.VerificationStatus)
                {
                    case UserVerificationStatus.Accepted:
                        responseDto.Message = $"Your business verification request has been " +
                            $"approved. The next time you log in, you should be verified.";
                    break;

                    case UserVerificationStatus.PendingEmail:
                        responseDto.Message = $"Your business verification request is pending " +
                            $"email confirmation. Please check your inbox periodically for " +
                            $"instructions.";
                    break;

                    case UserVerificationStatus.PendingAdmin:
                        responseDto.Message = $"Your verification request is pending review " +
                            $"by an admin. You will be notified once it's processed.";
                    break;

                    case UserVerificationStatus.Rejected:
                        responseDto.Message = $"Your verification request was rejected due " +
                            $"to your domains and business name not matching properly. " +
                            $"{errorMessageEnd}";
                    break;

                    default:
                        businessVerificationModel.ErrorOccured = true;
                        responseDto.Message = $"An unexpected error occured during your " +
                            $"business verification request process. Thus, your request has " +
                            $"not started yet. {errorMessageEnd}";
                    break;
                }

                // Execute writing to Firestore documents and returning a response
                await _firestoreService.SetDocumentByFirestorePath(
                        firestoreUserDocumentPath, userModel);
                await _firestoreService.SetDocumentByFirestorePath(
                    firestoreBusinessVerificationDocumentPath, businessVerificationModel);
                return responseDto;
            }
            // Handle unexpected errors gracefully
            catch
            {
                // Returning a response
                responseDto.Message = $"An unexpected error occured during your " +
                    $"business verification request process. {errorMessageEnd}";
                return responseDto;
            }
        }
    }
}
