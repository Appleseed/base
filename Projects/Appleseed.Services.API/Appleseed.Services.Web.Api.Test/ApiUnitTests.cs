namespace Appleseed.Services.Web.Api.Test
{
    using System.ServiceModel;

    using NUnit.Framework;

    [TestFixture]
    public class ApiUnitTests
    {
        private const string ValidAuthString = "BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E";

        private const string InValidAuthString = "BE7DC028-7238-45D3-AF35-DD3FE4AEFB7q";

        [Test]
        public void ValidateUser_WithValidPassword_ReturnsTrue()
        {
            // Arrange
            var client = new Appleseed.WebApi.PortalServicesClient();
            var header = new Appleseed.WebApi.AuthHeader { APIKey = ValidAuthString };

            // Act
            var result = client.ValidateUser(header, "admin@appleseedportal.net", "admin");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateUser_WithInvalidPassword_ReturnsFalse()
        {
            // Arrange
            var client = new Appleseed.WebApi.PortalServicesClient();
            var header = new Appleseed.WebApi.AuthHeader { APIKey = ValidAuthString };

            // Act
            var result = client.ValidateUser(header, "admin@appleseedportal.net", "admi");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        [ExpectedException(typeof(FaultException))]
        public void ValidateUser_WithInvalidApiKey_Fails()
        {
            // Arrange
            var client = new Appleseed.WebApi.PortalServicesClient();
            var header = new Appleseed.WebApi.AuthHeader { APIKey = InValidAuthString };
            try
            {
                // Act
                client.ValidateUser(header, "admin@appleseedportal.net", "admin");
            }
            catch(System.Exception ex)
            {
                // Invalid key exception will be thrown hence passed.
                Assert.Pass(ex.Message);
                return;
            }

            // Valid key so failed
            Assert.Fail();
            // Assert
            // Nothing to assert, exception will be captured
        }

        [Test]
        public void GetAllRoles_ReturnsValidRoles()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}
