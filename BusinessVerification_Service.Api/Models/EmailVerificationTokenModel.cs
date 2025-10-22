using System.ComponentModel.DataAnnotations;

namespace BusinessVerification_Service.Api.Models
{
    // Model representing an EmailVerificationToken document
    // from the EmailVerificationTokens collection in Firestore
    //
    // The Token field is used for identifying the specific Token document
    //
    // Certain fields are optional and may not be present in every document
    // or needed for every operation
    //
    // Certain string fields in Firestore should be converted
    // to enum types in this model
    public class EmailVerificationTokenModel
    {
        public string? Token { get; set; }

        public string? UserId { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
