using BusinessVerification_Service.Api.Interfaces.HelpersInterfaces;
using BusinessVerification_Service.Api.Models;

namespace BusinessVerification_Service.Api.Services
{
    public class EmailVerificationService
    {
        // Inject dependencies
        private readonly string _baseUrl;
        private readonly ITokenGeneratorHelper _tokenGeneratorHelper;

        // Constructor for dependency injection
        public EmailVerificationService(ServiceInformationModel serviceInformationModel,
            ITokenGeneratorHelper tokenGeneratorHelper)
        {
            _baseUrl = serviceInformationModel.baseUrl;
            _tokenGeneratorHelper = tokenGeneratorHelper;
        }

        // Process of sending a verification email upon request, receives a UserModel
        // and the relevant user ID
        //
        // The method is set as an async Task and not void so that errors
        // can be propogated correctly
        public async Task SendVerificationEmailProcess(UserModel user, string userId)
        {
            // Try catch wrapper
            try
            {
                // Create EmailVerificationTokenModel
                EmailVerificationTokenModel tokenModel = new()
                {
                    userId = userId,
                    email = user.email,
                    createdAt = DateTime.UtcNow,
                    expiresAt = DateTime.UtcNow.AddHours(24)
                };

                // Generate verification token
                string verificationToken = _tokenGeneratorHelper.GenerateToken();

                // Build link
                string verificationLink =
                    $"{_baseUrl}/api/EmailVerification/verify-email?verificationToken={verificationToken}";

                // Build email (method)

                // Send email (method)

                // Write EmailVerificationTokenModel to Firestore (method)
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
