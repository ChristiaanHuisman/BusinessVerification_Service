namespace BusinessVerification_Service.Api.Interfaces.HelpersInterfaces
{
    // Include methods from the helper
    public interface IDomainNameHelper
    {
        bool DomainExactMatch(string emailRegisterableDomain,
            string websiteRegisterableDomain);

        int FuzzyMatchScore(string variable1, string variable2);
    }
}
