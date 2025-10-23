using BusinessVerification_Service.Api.Models;
using System.Text.Json.Serialization;

namespace BusinessVerification_Service.Api.Dtos
{
    // For returning a response from the BusinessVerificationController
    // to the user Flutter app
    public class BusinessVerificationResponseDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserVerificationStatus VerificationStatus { get; set; }
            = UserVerificationStatus.NotStarted
        ;

        public string? Message { get; set; }
    }
}
