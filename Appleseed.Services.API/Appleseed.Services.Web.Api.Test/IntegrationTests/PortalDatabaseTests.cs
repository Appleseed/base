namespace Appleseed.Services.Web.Api.Test.IntegrationTests
{
    using System.Configuration;
    using System.Transactions;

    using Appleseed.Portal.Core.Exceptions;
    using Appleseed.Portal.Core.Models;
    using Appleseed.Portal.Database.Context;

    using Common.Logging;

    using Moq;

    using NUnit.Framework;

    using Ploeh.AutoFixture;

    [TestFixture]
    public class PortalDatabaseTests
    {
        private ILog log;

        private PortalDatabase database;

        [SetUp]
        public void Setup()
        {
            this.log = new Mock<ILog>().Object;
            this.database = new PortalDatabase(this.log, ConfigurationManager.ConnectionStrings["AppleseedConnectionString"].ConnectionString);
        }


        [Test]
        public void CreatePortal_ExpectValidPortalId()
        {
            // Arrange
            var fixture = new Fixture();
            var item = fixture.Create<Portal>();
            item.PortalId = 0;
            
            // Act
            using (var scope = new TransactionScope())
            {
                this.database.CreatePortal(item);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.IsTrue(item.PortalId > 0);
        }

        // This test fails due to an issue inside the sproc
        [Test]
        public void DeletePortal_ExpectNoErrors()
        {
            // Arrange

            // Act
            using (var scope = new TransactionScope())
            {
                this.database.DeletePortal(0);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.Pass();
        }

        [Test]
        [ExpectedException(typeof(AppleseedDatabaseException))]
        public void DeletePortal_ExpectError()
        {
            // Arrange

            // Act
            using (var scope = new TransactionScope())
            {
                this.database.DeletePortal(0);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.Pass();
        }
    }
}
