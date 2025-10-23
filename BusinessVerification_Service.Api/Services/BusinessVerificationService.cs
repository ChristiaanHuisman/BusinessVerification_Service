using BusinessVerification_Service.Api.Dtos;
using Nager.PublicSuffix;
using System.Net.Mail;

namespace BusinessVerification_Service.Api.Services
{
    public class BusinessVerificationService
    {
        // Inject dependencies
        private readonly IDomainParser _domainParser;

        // Constructor for dependency injection
        public BusinessVerificationService(IDomainParser domainParser)
        {
            _domainParser = domainParser;
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
