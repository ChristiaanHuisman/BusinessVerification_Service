namespace BusinessVerification_Service.Api.Dtos
{
    // For returning parsed doamin info
    public class ParsedDomainDto
    {
        public string? RegistrableDomain { get; set; }

        public string? TopLevelDomain { get; set; }

        public string? Domain { get; set; }
    }
}
