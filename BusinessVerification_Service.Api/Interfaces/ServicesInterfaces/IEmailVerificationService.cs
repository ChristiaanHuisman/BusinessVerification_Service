using BusinessVerification_Service.Api.Models;

namespace BusinessVerification_Service.Api.Interfaces.ServicesInterfaces
{
    // Include methods from the service
    public interface IEmailVerificationService
    {
        Task SendVerificationEmailProcess(UserModel userModel, string userId);
    }
}
