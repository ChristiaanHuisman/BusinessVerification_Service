﻿using BusinessVerification_Service.Api.Dtos;
using BusinessVerification_Service.Api.Services;
using BusinessVerification_Service.Test.Fixtures;
using Nager.PublicSuffix;

namespace BusinessVerification_Service.Test.ServicesTests
{
    [Trait("Catagory", "BusinessVerificationService Integration Testing")]
    public class BusinessVerificationServiceTest : IClassFixture<DomainParserFixture>
    {
        // Injected dependencies
        private readonly IDomainParser _domainParser;

        // Constructor for dependency injection
        public BusinessVerificationServiceTest(
            DomainParserFixture fixture)
        {
            _domainParser = fixture.DomainParser;
        }

        // Helper method to create the service
        private BusinessVerificationService CreateService()
        {
            return new BusinessVerificationService(_domainParser,
                // // Other service dependencies are not needed for this test
                null, null, null, null, null
            );
        }

        // Test domain parsing with valid email and website addresses
        [Theory]
        // Arrange
        [InlineData("test@example.com", "https://example.com",
            "example.com", "com", "example")]
        [InlineData("test+alaias@example.com", "https://example.com",
            "example.com", "com", "example")]
        [InlineData("test@user.example.co.za", "https://example.co.za/path",
            "example.co.za", "co.za", "example")]
        public void GetDomainInfo_ValidEmailAndWebsite_ReturnsParsedDto(
            string emailInput, string websiteInput,
            string expectedRegisterable, string expectedTopLevel, string exectedDomain)
        {
            // Also arrange
            BusinessVerificationService service = CreateService();

            // Act
            (ParsedDomainDto? parsedEmailDomain, ParsedDomainDto? parsedWebsiteDomain)
                = service.GetDomainInfo(emailInput, websiteInput);

            // Assert
            Assert.NotNull(parsedEmailDomain);
            Assert.Equal(expectedRegisterable, parsedEmailDomain.RegistrableDomain);
            Assert.Equal(expectedTopLevel, parsedEmailDomain.TopLevelDomain);
            Assert.Equal(exectedDomain, parsedEmailDomain.Domain);
            Assert.NotNull(parsedWebsiteDomain);
            Assert.Equal(expectedRegisterable, parsedWebsiteDomain.RegistrableDomain);
            Assert.Equal(expectedTopLevel, parsedWebsiteDomain.TopLevelDomain);
            Assert.Equal(exectedDomain, parsedWebsiteDomain.Domain);
        }

        // Test domain parsing with invalid email and website addresses
        [Theory]
        // Arrange
        [InlineData("test@example.com", "https://example")]
        [InlineData("test@example.com", "example.com")]
        [InlineData("test@example", "https://example.co.za")]
        [InlineData("test@example.co.za", "https://.co.za")]
        public void GetDomainInfo_WebsiteWithoutTld_ReturnsNulls(
            string emailInput, string websiteInput)
        {
            // Also arrange
            BusinessVerificationService service = CreateService();

            // Act
            (ParsedDomainDto? parsedEmailDomain, ParsedDomainDto? parsedWebsiteDomain)
                = service.GetDomainInfo(emailInput, websiteInput);

            // Assert
            Assert.Null(parsedEmailDomain);
            Assert.Null(parsedWebsiteDomain);
        }
    }
}
