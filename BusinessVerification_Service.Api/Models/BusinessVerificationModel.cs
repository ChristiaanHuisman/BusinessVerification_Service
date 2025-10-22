using System.Text.Json.Serialization;

namespace BusinessVerification_Service.Api.Models
{
    // Model representing a BusinessVerification document within
    // a User document of the Users collection in Firestore
    //
    // The UserId field is used for identifying
    // the BusinessVerification document of a specific User docment
    //
    // Certain fields are optional and may not be present in every document
    // or needed for every operation
    //
    // Certain string fields in Firestore should be converted
    // to enum types in this model
    public class BusinessVerificationModel
    {
        public string? UserId { get; set; }

        public int? Attempt { get; set; }

        public bool? ErrorOccured { get; set; }

        public bool? EmailVerified { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserVerificationStatus? VerificationStatus { get; set; }

        public int? FuzzyScore { get; set; }

        public DateTime? VerificationRequestedAt { get; set; }

        public DateTime? VerificationStatusUpdatedAt { get; set; }
    }
}
