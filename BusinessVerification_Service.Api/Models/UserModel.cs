using System.Text.Json.Serialization;

namespace BusinessVerification_Service.Api.Models
{
    // Model representing a User document from the Users collection
    // in Firestore
    //
    // The UserId field is used for identifying the specific User document
    //
    // Certain fields are optional and may not be present in every document
    // or needed for every operation
    //
    // Certain string fields in Firestore should be converted
    // to enum types in this model
    public class UserModel
    {
        public string? UserId { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Website { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole? Role { get; set; } = UserRole.Customer;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserVerificationStatus? VerificationStatus { get; set; }
            = UserVerificationStatus.NotStarted
        ;

        public DateTime? VerificationRequestedAt { get; set; }
            = DateTime.UtcNow
        ;

        public bool? EmailVerified { get; set; } = false;
    }

    // Enum declaring

    public enum UserRole
    {
        Customer,
        Business,
        Admin
    }

    public enum UserVerificationStatus
    {
        NotStarted,
        PendingAdmin,
        PendingEmail,
        Rejected,
        Accepted
    }
}
