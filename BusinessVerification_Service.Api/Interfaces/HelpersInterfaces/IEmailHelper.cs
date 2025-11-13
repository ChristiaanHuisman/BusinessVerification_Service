namespace BusinessVerification_Service.Api.Interfaces.HelpersInterfaces
{
    // Include methods from the helper
    public interface IEmailHelper
    {
        string BuildVerificationEmailHtml(string businessName, string verificationLink);
    }
}
