namespace Appleseed.Services.Web.Api.Test.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Transactions;

    using Appleseed.Portal.Core.Models;
    using Appleseed.Portal.Database.Context;

    using Common.Logging;

    using Moq;

    using NUnit.Framework;

    using Ploeh.AutoFixture;

    [TestFixture]
    public class HtmlTextDatabaseTests
    {
        private ILog log;

        private ModuleDatabase database;

        [SetUp]
        public void Setup()
        {
            this.log = new Mock<ILog>().Object;
            this.database = new ModuleDatabase(this.log, ConfigurationManager.ConnectionStrings["AppleseedConnectionString"].ConnectionString, "Appleseed", 0);
        }

        [Test, Category("Integration")]
        public void CreateHtmlText_ExpectValidId()
        {
            // Arrange
            var fixture = new Fixture();
            var item = fixture.Create<HtmlText>();
            item.ModuleId = 1;

            // Act
            using (var scope = new TransactionScope())
            {
                try
                {
                    // Actr
                    this.database.CreateHtmlText(item);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.IsTrue(item.ModuleId > 0);
        }

        [Test, Category("Integration")]
        public void UpdateHtmlText_ExpectValidId()
        {
            // Arrange
            int result = 0;
            int expected = 1;
            HtmlText item;

            try
            {
                item = this.database.RetrieveHtmlText(2);
            }
            catch (Exception ex)
            {
                this.log.Error(ex);
                throw;
            }

            item.DesktopHtml = "Some new text";

            // Act
            using (var scope = new TransactionScope())
            {
                try
                {
                    // Actr
                    result = this.database.UpdateHtmlText(item);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.AreEqual(expected, result);
        }

        // This test fails due to an issue inside the sproc
        [Test, Category("Integration")]
        public void DeleteHtmlText_ExpectNoErrors()
        {
            // Arrange
            var expected = 1;
            int affectedRecordCount = 0;

            // Act
            using (var scope = new TransactionScope())
            {
                affectedRecordCount = this.database.DeleteHtmlText(2,1);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.AreEqual(expected, affectedRecordCount);
        }
    }
}
