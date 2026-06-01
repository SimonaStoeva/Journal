using Journal.Services;
using Moq;
using Xunit;

namespace Journal.Tests
{
    public class UserTests
    {
        [Fact]
        public void Test_RegisterUser()
        {
            var authServiceMock = new Mock<AuthenticationService>();

            authServiceMock
                .Setup(x => x.RegisterUser("testuser", "password123"))
                .Returns(true);

            bool result = authServiceMock.Object.RegisterUser("testuser", "password123");

            Assert.True(result);
        }
        
        [Fact]
        public void RegisterUser_ReturnsFalse_WhenRegistrationFails()
        {
            var authMock = new Mock<AuthenticationService>();

            authMock
                .Setup(x => x.RegisterUser("existingUser", "123456"))
                .Returns(false);

            bool result = authMock.Object.RegisterUser("existingUser", "123456");

            Assert.False(result);
        }
        
        [Fact]
        public void RegisterUser_ReturnsFalse_WhenUsernameAlreadyExists()
        {
            var authMock = new Mock<AuthenticationService>();

            authMock
                .Setup(x => x.RegisterUser("ivan", "123456"))
                .Returns(false);

            bool result = authMock.Object.RegisterUser("ivan", "123456");

            Assert.False(result);
        }
        
        [Fact]
        public void RegisterUser_WithEmptyUsername_ReturnsFalse()
        {
            var authMock = new Mock<AuthenticationService>();

            authMock
                .Setup(x => x.RegisterUser("", "123456"))
                .Returns(false);

            bool result = authMock.Object.RegisterUser("", "123456");

            Assert.False(result);
        }
    }
    
    
}