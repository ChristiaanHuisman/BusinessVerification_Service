using BusinessVerification_Service.Api.Helpers;
using Xunit.Abstractions;

namespace BusinessVerification_Service.Test.HelpersTests
{
    [Trait("Catagory", "DomainNameHelper Test")]
    public class DomainNameHelperTest
    {
        // Injected dependencies
        private readonly DomainNameHelper _helper;
        private readonly ITestOutputHelper _output;

        // Constructor for dependency injection
        public DomainNameHelperTest(ITestOutputHelper output)
        {
            _helper = new DomainNameHelper();
            _output = output;
        }

        // Test for a high fuzzy score
        [Fact]
        public void FuzzyMatchScore_ShouldBeHigh()
        {
            // Arrange
            string a = "test example";
            string b = "tst exmple";

            // Act
            int score = _helper.FuzzyMatchScore(a, b);
            _output.WriteLine($"Score: {score}");

            // Assert
            Assert.True(score >= 75 && score <= 100, $"Expected high score, got: {score}");
        }

        // Tes for a medium fuzzy score
        [Fact]
        public void FuzzyMatchScore_ShouldBeMedium()
        {
            // Arrange
            string a = "test example";
            string b = "example string for testing purposes";

            // Act
            int score = _helper.FuzzyMatchScore(a, b);
            _output.WriteLine($"Score: {score}");

            // Assert
            Assert.True(score >= 35 && score <= 65, $"Expected high score, got: {score}");
        }

        // Test for a low fuzzy score
        [Fact]
        public void FuzzyMatchScore_ShouldBeLow()
        {
            // Arrange
            string a = "test example";
            string b = "different";

            // Act
            int score = _helper.FuzzyMatchScore(a, b);
            _output.WriteLine($"Score: {score}");

            // Assert
            Assert.True(score >= 0 && score <= 25, $"Expected low score, got: {score}");
        }
    }
}
