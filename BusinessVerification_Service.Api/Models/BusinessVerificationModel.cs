using System.Text.Json.Serialization;

namespace BusinessVerification_Service.Api.Models
{
    // Model representing a BusinessVerification document within
    // a User document of the Users collection in Firestore
    //
    // Certain fields are optional and may not be present in every document
    // or needed for every operation
    //
    // Certain string fields in Firestore should be converted
    // to enum types in this model
    public class BusinessVerificationModel
    {
        // Automatically increment attempt number whenever the model
        // is initialized
        private int _AttemptNumber = 0;
        public int AttemptNumber
        {
            get => _AttemptNumber;
            set => _AttemptNumber = value + 1;
        }

        public bool? ErrorOccured { get; set; } = false;

        public bool? EmailVerified { get; set; } = false;

        // Automatically update the given timestamp when the verification
        // status changes of enum value
        private UserVerificationStatus _VerificationStatus =
            UserVerificationStatus.NotStarted;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserVerificationStatus VerificationStatus
        {
            get => _VerificationStatus;
            set
            {
                if (_VerificationStatus != value)
                {
                    _VerificationStatus = value;
                    VerificationStatusUpdatedAt = DateTime.UtcNow;
                }
            }
        }

        public int? FuzzyScore { get; set; }

        public DateTime? VerificationRequestedAt { get; set; }

        // Only updated internally
        public DateTime? VerificationStatusUpdatedAt { get; private set; }


        // Helper methods

        public void SetVerificationStatus(UserModel userModel)
        {
            VerificationStatus = userModel.VerificationStatus
                ?? UserVerificationStatus.NotStarted;
        }

        public void SetEmailVerified(UserModel userModel)
        {
            EmailVerified = userModel.EmailVerified
                ?? false;
        }

        public void SetVerificationRequestedAt(UserModel userModel)
        {
            VerificationRequestedAt = userModel.VerificationRequestedAt;
        }
    }
}
