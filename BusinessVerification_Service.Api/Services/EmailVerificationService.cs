using BusinessVerification_Service.Api.Dtos;
using BusinessVerification_Service.Api.Interfaces.HelpersInterfaces;
using BusinessVerification_Service.Api.Interfaces.ServicesInterfaces;
using BusinessVerification_Service.Api.Models;

namespace BusinessVerification_Service.Api.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        // Inject dependencies
        private readonly string _baseUrl;
        private readonly ITokenGeneratorHelper _tokenGeneratorHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IFirestoreService _firestoreService;

        // Constructor for dependency injection
        public EmailVerificationService(ServiceInformationModel serviceInformationModel,
            ITokenGeneratorHelper tokenGeneratorHelper,
            IEmailHelper emailHelper,
            IFirestoreService firestoreService)
        {
            _baseUrl = serviceInformationModel.baseUrl;
            _tokenGeneratorHelper = tokenGeneratorHelper;
            _emailHelper = emailHelper;
            _firestoreService = firestoreService;
        }

        // Standard error message ending for displaying user error messages
        const string errorMessageEnd = "Please ensure all account details are correct " +
            "and try again in a few minutes, contact support if the issue persists.";

        // Collection names in Firestore
        const string emailVerificationTokenCollection = "emailVerificationTokens";

        // When a user requests a verification email that has not requested one before, the
        // method receives the relevant UserModel and userId
        public async Task NewVerificationEmail(UserModel userModel, string userId)
        {
            // Create EmailVerificationTokenModel
            EmailVerificationTokenModel tokenModel = new()
            {
                userId = userId,
                name = userModel.name,
                email = userModel.email,
            };
            tokenModel.SetTokenTimestamps();

            // Send the verification email
            await SendVerificationEmailProcess(tokenModel);
        }

        // When a user requests to resend a verification email, the method receives
        // the relevant old EmailVerificationTokenModel and token self
        public async Task ResendVerificationEmail(EmailVerificationTokenModel oldTokenModel,
            string oldVerificationToken)
        {
            // Create new EmailVerificationTokenModel
            EmailVerificationTokenModel tokenModel = new()
            {
                userId = oldTokenModel.userId,
                name = oldTokenModel.name,
                email = oldTokenModel.email,
            };
            tokenModel.SetTokenTimestamps();

            // // Send the verification email
            await SendVerificationEmailProcess(tokenModel);

            // Get relevant Firebase document paths
            string firestoreEmailVerificationTokenDocumentPath =
                $"{emailVerificationTokenCollection}/{oldVerificationToken}";

            // Delete old email verification token from Firestore
            await _firestoreService.DeleteDocumentFromFirestore(
                firestoreEmailVerificationTokenDocumentPath);
        }

        // Process of sending a verification email upon request, receives
        // an EmailVerificationTokenModel
        //
        // The method is set as an async Task and not void so that errors
        // can be propogated correctly
        public async Task SendVerificationEmailProcess(EmailVerificationTokenModel tokenModel)
        {
            // Try catch wrapper
            try
            {
                // Generate verification token
                string verificationToken = _tokenGeneratorHelper.GenerateToken();

                // Build link
                string verificationLink =
                    $"{_baseUrl}/api/EmailVerification/verify-email?verificationToken={verificationToken}";

                // Build email content
                string emailSubject = "Verify your EngagePoint account email address";
                string emailHtml = _emailHelper.BuildVerificationEmailHtml(
                    tokenModel.name, verificationLink);

                // Send email via SMTP
                await _emailHelper.SendEmailSmtp(tokenModel.email, tokenModel.name,
                    emailSubject, emailHtml);

                // Get relevant Firebase document paths
                string firestoreEmailVerificationTokenDocumentPath =
                    $"{emailVerificationTokenCollection}/{verificationToken}";

                // Write EmailVerificationTokenModel to Firestore
                await _firestoreService.SetDocumentByFirestorePath(
                    firestoreEmailVerificationTokenDocumentPath, tokenModel);
            }
            // Log error
            catch (Exception exception)
            {
                Console.WriteLine($"Failed to send verification email: {exception.Message}");
                throw;
            }
        }

        // Process of verifying the email verification token when a user clicks on the verification
        // link, receives the verification token and returns a message to display to the user
        public async Task<BusinessVerificationResponseDto> VerifyEmailVerificaitonToken(
            string? verificaitonToken)
        {
            // Create response DTO instance
            BusinessVerificationResponseDto responseDto = new();

            try
            {

            }
            // Handle unexpected errors gracefully
            catch (Exception exception)
            {
                // Returning a response
                responseDto.message = $"An unexpected error occured during your " +
                    $"email verification process. Request the resending of your " +
                    $"verification email in the EngagePoint mobile applicaiton. {errorMessageEnd}";
                Console.WriteLine($"Failed process business verification: {exception.Message}");
                return responseDto;
            }
        }
    }
}
