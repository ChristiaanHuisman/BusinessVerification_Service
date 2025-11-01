using Google.Cloud.Firestore;

namespace BusinessVerification_Service.Api.Models
{
    // Model representing a User document from Firestore
    [FirestoreData]
    public class UserModel
    {
        public UserModel() { }

        [FirestoreProperty]
        public string? Name { get; set; }

        [FirestoreProperty]
        public string? Email { get; set; }

        [FirestoreProperty]
        public string? Website { get; set; }

        // Non-nullable enum, Firestore will store as string
        [FirestoreProperty(ConverterType = typeof(
            Services.FirestoreService.FirestoreEnumStringConverter<UserRole>))]
        public UserRole Role { get; set; } = UserRole.Customer;

        // Enum stored as string
        [FirestoreProperty(ConverterType = typeof(
            Services.FirestoreService.FirestoreEnumStringConverter<UserVerificationStatus>))]
        public UserVerificationStatus VerificationStatus { get; set; }
            = UserVerificationStatus.NotStarted;

        // Nullable timestamp
        [FirestoreProperty]
        public DateTime? VerificationRequestedAt { get; set; }

        [FirestoreProperty]
        public bool? EmailVerified { get; set; } = false;
    }

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

