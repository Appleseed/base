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
    public class ModuleDatabaseTests
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
        public void CreatePageModule_ExpectValidId()
        {
            // Arrange
            var fixture = new Fixture();
            var item = fixture.Create<PageModule>();
            item.ModuleID = 0;
            item.TabID = 100;
            item.ModuleDefID = 1;
            
            // Act
            using (var scope = new TransactionScope())
            {
                try
                {
                    // Actr
                    this.database.CreatePageModule(item);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.IsTrue(item.ModuleID > 0);
        }

        [Test, Category("Integration")]
        public void RetrieveAllPageModule_ExpectValidResults()
        {
            // Arrange
            var pageId = 100;
            List<PageModule> results = null;

            // Act
                try
                {
                    // Actr
                    results = this.database.RetrieveAllPageModules(pageId);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }

            // Assert
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count > 0);
        }

        [Test, Category("Integration")]
        public void UpdatePageModule_ExpectValidId()
        {
            // Arrange
            int result = 0;
            int expected = 1;
            PageModule item;

            try
            {
                item = this.database.RetrievePageModule(1);
            }
            catch (Exception ex)
            {
                this.log.Error(ex);
                throw;
            }

            item.AuthorizedPropertiesRoles = item.AuthorizedPropertiesRoles ?? string.Empty;
            item.AuthorizedPublishingRoles = item.AuthorizedPublishingRoles ?? string.Empty;
            item.AuthorizedMoveModuleRoles = item.AuthorizedMoveModuleRoles ?? string.Empty;
            item.AuthorizedDeleteModuleRoles = item.AuthorizedDeleteModuleRoles ?? string.Empty;
            item.AuthorizedApproveRoles = item.AuthorizedApproveRoles ?? string.Empty;

            item.PaneName = "PaneName";

            // Act
            using (var scope = new TransactionScope())
            {
                try
                {
                    // Actr
                    result = this.database.UpdatePageModule(item);
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
        public void DeletePageModule_ExpectNoErrors()
        {
            // Arrange
            var expected = 1;
            int affectedRecordCount = 0;

            // Act
            using (var scope = new TransactionScope())
            {
                affectedRecordCount = this.database.DeletePageModule(1);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.AreEqual(expected, affectedRecordCount);
        }

        [Test, Category("Integration")]
        public void RetrievePage_ExpectValidResults()
        {
            // Arrange
            var pageId = 100;
            Page result = null;

            // Act
            try
            {
                // Actr
                result = this.database.RetrievePage(pageId);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            // Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.PageID > 0);
        }

        [Test, Category("Integration")]
        public void RetrieveAllPages_ExpectValidResults()
        {
            // Arrange
            var portalId = 0;
            List<Page> result = null;

            // Act
            try
            {
                // Actr
                result = this.database.RetrieveAllPages(portalId);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            // Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count > 0);
        }
    }
}
