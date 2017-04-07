namespace Appleseed.Services.Web.Api.Test
{
    using System.Linq;

    using Appleseed.Portal.Core.Models;
    using Appleseed.Portal.Domain.Services;

    using NUnit.Framework;

    [TestFixture]
    public class ValidationTests
    {
        [Test]
        public void Validation_Validate_ReturnsErrors()
        {
            // Arrange
            var role = new AsRole();

            // Act
            var result = Validation.Validate(role);

            // Assert
            Assert.IsTrue(result.Any());
        }
    }
}
