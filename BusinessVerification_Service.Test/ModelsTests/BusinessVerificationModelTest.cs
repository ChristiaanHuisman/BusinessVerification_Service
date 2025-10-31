using BusinessVerification_Service.Api.Models;

namespace BusinessVerification_Service.Test.ModelsTests
{
    [Trait("Catagory", "BusinessVerificationModel Unit Testing")]
    public class BusinessVerificationModelTest
    {
        // Test automatic incrementing of attempt number
        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        public void AttemptNumber_IncrementsOnSet(int initial, int next)
        {
            // Arrange
            BusinessVerificationModel model = new BusinessVerificationModel();

            // Act
            model.AttemptNumber = initial;

            // Assert
            Assert.Equal(next, model.AttemptNumber);
        }

        // Test when verification status changes
        [Fact]
        public void VerificationStatus_UpdatesPropertyAndTimestamp()
        {
            // Arrange
            BusinessVerificationModel model = new BusinessVerificationModel();
            DateTime? oldTimestamp = DateTime.UtcNow;
            Thread.Sleep(1000);

            // Act
            model.VerificationStatus = UserVerificationStatus.PendingAdmin;

            // Assert
            Assert.Equal(UserVerificationStatus.PendingAdmin, model.VerificationStatus);
            Assert.NotNull(model.VerificationStatusUpdatedAt);
            Assert.True(model.VerificationStatusUpdatedAt > oldTimestamp);
        }

        // Test when verification status stays the same
        [Fact]
        public void VerificationStatus_DoesNotUpdateTimestamp()
        {
            // Arrange
            BusinessVerificationModel model = new BusinessVerificationModel();
            model.VerificationStatus = UserVerificationStatus.NotStarted;
            DateTime? oldTimestamp = model.VerificationStatusUpdatedAt;

            // Act
            model.VerificationStatus = UserVerificationStatus.NotStarted;

            // Assert
            Assert.Equal(UserVerificationStatus.NotStarted, model.VerificationStatus);
            Assert.Equal(oldTimestamp, model.VerificationStatusUpdatedAt);
        }

        // Test verification status changes when helper method is called
        [Fact]
        public void SetVerificationStatus_UpdatesVerificationStatus()
        {
            // Arrange
            UserModel user = new UserModel { VerificationStatus
                = UserVerificationStatus.Accepted };
            BusinessVerificationModel model = new BusinessVerificationModel();

            // Act
            model.SetVerificationStatus(user);

            // Assert
            Assert.Equal(UserVerificationStatus.Accepted, model.VerificationStatus);
            Assert.NotNull(model.VerificationStatusUpdatedAt);
        }

        // Test email verification status changes when helper method is called
        [Fact]
        public void SetEmailVerified_UpdatesEmailVerified()
        {
            // Arrange
            UserModel user = new UserModel { EmailVerified = true };
            BusinessVerificationModel model = new BusinessVerificationModel();

            // Act
            model.SetEmailVerified(user);

            // Assert
            Assert.True(model.EmailVerified);
        }

        // Test verification requested at changes when helper method is called
        [Fact]
        public void SetVerificationRequestedAt_UpdatesTimestamp()
        {
            // Arrange
            UserModel user = new UserModel { VerificationRequestedAt = DateTime.UtcNow };
            BusinessVerificationModel model = new BusinessVerificationModel();

            // Act
            model.SetVerificationRequestedAt(user);

            // Assert
            Assert.Equal(user.VerificationRequestedAt, model.VerificationRequestedAt);
        }
    }
}
