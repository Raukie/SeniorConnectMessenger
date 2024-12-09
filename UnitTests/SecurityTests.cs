using DataAccessExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class DataHelperTests
    {
        [TestMethod]
        public void ShouldHashPasswordCorrectly()
        {
            // Arrange
            string plainPassword = "TestPassword123";

            // Act
            string hashedPassword = DataHelper.HashPassword(plainPassword);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(hashedPassword));
            Assert.AreNotEqual(plainPassword, hashedPassword);
        }

        [TestMethod]
        public void ShouldVerifyPasswordSuccessfully_WithValidPlainPassword()
        {
            // Arrange

            string plainPassword = "TestPassword123";
            string hashedPassword = DataHelper.HashPassword(plainPassword);

            // Act

            bool isValid = DataHelper.Verify(hashedPassword, plainPassword);

            // Assert

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void ShouldFailVerification_WithInvalidPlainPassword()
        {
            // Arrange

            string plainPassword = "TestPassword123";
            string invalidPassword = "WrongPassword";
            string hashedPassword = DataHelper.HashPassword(plainPassword);

            // act

            bool isValid = DataHelper.Verify(hashedPassword, invalidPassword);

            // Assert

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ShouldFailVerification_WithInvalidHash()
        {
            // Arrange

            string plainPassword = "TestPassword123";
            string invalidHashedPassword = "$2a$12$InvalidHashThatDoesNotWork";

            // Act

            bool isValid = DataHelper.Verify(invalidHashedPassword, plainPassword);

            // Assert

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ShouldGenerateUniqueHashes_ForSamePlainPassword()
        {
            // Arrenge

            string plainPassword = "TestPassword123";

            // Act

            string hashedPassword1 = DataHelper.HashPassword(plainPassword);
            string hashedPassword2 = DataHelper.HashPassword(plainPassword);

            // Assert

            Assert.AreNotEqual(hashedPassword1, hashedPassword2);
        }
    }
}
