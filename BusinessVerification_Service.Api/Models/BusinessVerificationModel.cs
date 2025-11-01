using Google.Cloud.Firestore;

namespace BusinessVerification_Service.Api.Models
{
    // Model representing a BusinessVerification document
    [FirestoreData]
    public class BusinessVerificationModel
    {
        public BusinessVerificationModel() { }

        // Automatically increment attempt number whenever the model
        // is initialized
        [FirestoreProperty]
        public int AttemptNumber { get; set; } = 0;

        [FirestoreProperty]
        public bool? ErrorOccured { get; set; } = false;

        [FirestoreProperty]
        public bool? EmailVerified { get; set; } = false;

        // Enum stored as string
        [FirestoreProperty(ConverterType = typeof(Services.FirestoreService.FirestoreEnumStringConverter<UserVerificationStatus>))]
        public UserVerificationStatus VerificationStatus { get; set; } = UserVerificationStatus.NotStarted;

        [FirestoreProperty]
        public int? FuzzyScore { get; set; }

        [FirestoreProperty]
        public DateTime? VerificationRequestedAt { get; set; }

        // Only updated internally
        [FirestoreProperty]
        public DateTime? VerificationStatusUpdatedAt { get; set; }

        // Helper methods
        public void SetVerificationStatus(UserModel userModel)
        {
            VerificationStatus = userModel.VerificationStatus;
            VerificationStatusUpdatedAt = DateTime.UtcNow;
        }

        public void SetEmailVerified(UserModel userModel)
        {
            EmailVerified = userModel.EmailVerified ?? false;
        }

        public void SetVerificationRequestedAt(UserModel userModel)
        {
            VerificationRequestedAt = userModel.VerificationRequestedAt;
        }
    }
}

