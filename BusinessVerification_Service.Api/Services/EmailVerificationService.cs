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

        // Collection names in Firestore
        const string emailVerificationTokenCollection = "emailVerificationTokens";
        
        // Process of sending a verification email upon request, receives a UserModel
        // and the relevant user ID
        //
        // The method is set as an async Task and not void so that errors
        // can be propogated correctly
        public async Task SendVerificationEmailProcess(UserModel userModel, string userId)
        {
            // Try catch wrapper
            try
            {
                // Create EmailVerificationTokenModel
                EmailVerificationTokenModel tokenModel = new()
                {
                    userId = userId,
                    email = userModel.email,
                    createdAt = DateTime.UtcNow,
                    expiresAt = DateTime.UtcNow.AddHours(24)
                };

                // Generate verification token
                string verificationToken = _tokenGeneratorHelper.GenerateToken();

                // Build link
                string verificationLink =
                    $"{_baseUrl}/api/EmailVerification/verify-email?verificationToken={verificationToken}";

                // Build email content
                string emailSubject = "Verify your EngagePoint account email address";
                string emailHtml = _emailHelper.BuildVerificationEmailHtml(
                    userModel.name, verificationLink);

                // Send email via SMTP
                await _emailHelper.SendEmailSmtp(userModel.email,userModel.name,
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
    }
}
