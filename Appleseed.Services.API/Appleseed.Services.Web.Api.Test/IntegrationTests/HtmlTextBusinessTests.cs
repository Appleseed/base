namespace Appleseed.Services.Web.Api.Test.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Transactions;
    using System.Linq;
    using Appleseed.Portal.Core.Models;
    using Appleseed.Portal.Database.Context;
    using Appleseed.Portal.Domain.Services;

    using Common.Logging;

    using Moq;

    using NUnit.Framework;

    using Ploeh.AutoFixture;

    [TestFixture]
    public class HtmlTextBusinessTests
    {
        private ILog log;

        private ModuleDomain context;

        [SetUp]
        public void Setup()
        {
            this.log = new Mock<ILog>().Object;
            var database = new ModuleDatabase(this.log, ConfigurationManager.ConnectionStrings["AppleseedConnectionString"].ConnectionString, "Appleseed", 0);
            this.context = new ModuleDomain(this.log, database);
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
                    this.context.CreateHtmlText(item);
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

        // TODO: Martin to test
        [Test, Category("Integration")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void CreateHtmlText_GivenNullData_ExpectError()
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
                    this.context.CreateHtmlText(null);
                }
                catch (Exception ex)
                {
                    Assert.Pass(ex.Message);
                }

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.IsTrue(item.ModuleId.Equals(0));
        }


        // TODO: Martin Need to Test
        // TODO; Identify ExpectedExpection
        //[ExpectedException(typeof(System.ArgumentNullException))]
        [Test, Category("Integration")]
        public void CreateHtmlText_GivenInvalidData_ExpectValidationErrors()
        {
            var fixture = new Fixture();
            var item = new HtmlText();

            // Act
            using (var scope = new TransactionScope())
            {
                try
                {
                    // Act
                    var result = this.context.CreateHtmlText(item);
                    Assert.IsTrue(result.Count > 0);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.IsTrue(item.ModuleId.Equals(0));


        }

        // TODO: Martin Need to Test
        // TODO: Identify ExpectedExpection
        //[ExpectedException(typeof(System.ArgumentNullException))]
        [Test, Category("Integration")]
        public void UpdateHtmlText_ExpectValidId()
        {
            // Arrange
            HtmlText item;
            List<ValidationResult> result = null;

            try
            {
                item = this.context.RetrieveHtmlText(2);
            }
            catch (Exception ex)
            {
                this.log.Error(ex);
                throw;
            }

            item.MobileDetails = "Some new text";
            item.MobileSummary = "Some new text";

            // Act
            using (var scope = new TransactionScope())
            {
                try
                {
                    result = this.context.UpdateHtmlText(item);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.IsNotNull(result);
            // Updated text will return 2
            Assert.IsTrue(result.Count.Equals(2));
        }


        // TODO: Martin Need to Test
        [Test, Category("Integration")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void UpdateHtmlText_GivenNullData_ExpectError()
        {
            // Arrange
            var fixture = new Fixture();
            List<ValidationResult> result = null;

            // Act
            using (var scope = new TransactionScope())
            {
                try
                {
                    // Actr
                    result = this.context.UpdateHtmlText(null);

                }
                catch (Exception ex)
                {
                    Assert.Pass(ex.Message);
                }

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsTrue(result.Count > 0);

        }

        // TODO: Martin Need to Test
        // TODO: Identify ExpectedExpection
        //[ExpectedException(typeof(System.ArgumentNullException))]
        // This test fails due to an issue inside the sproc
        [Test, Category("Integration")]
        public void DeleteHtmlText_ExpectNoErrors()
        {
            // Arrange
            List<ValidationResult> result = null;

            // Act
            using (var scope = new TransactionScope())
            {
                result = this.context.DeleteHtmlText(2,1);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count.Equals(0));
        }

        [Test, Category("Integration")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void DeleteHtmlText_GivenInvalidId_ExpectError()
        {
            // Arrange

            // Act
            this.context.DeleteHtmlText(0,0);

            // Assert
        }

        [Test, Category("Integration")]
        public void GetHtmlByVersion_ExpectValidResult()
        {
            HtmlText htmltext = null;
            // Get html based on version number
            using (var scope = new TransactionScope())
            {
                var fixture = new Fixture();
                var item = fixture.Create<HtmlText>();
                item.ModuleId = 5;
                item.VersionNo = 2;

                this.context.CreateHtmlVersionText(item);

                // now getting html by version which is created above
                htmltext = this.context.GetHtmlByVersion(item.ModuleId, item.VersionNo);

                scope.Dispose();
            }
            // Expected value should not be null becasue we get html text which is newly created
            Assert.AreNotEqual(null, htmltext);
        }

        [Test, Category("Integration")]
        public void CreateNewHtmlVersion_ExpectValidResult()
        {
            // create new version 
            int result = 1;
            using (var scope = new TransactionScope())
            {
                var fixture = new Fixture();
                var item = fixture.Create<HtmlText>();
                item.ModuleId = 5;

                result = this.context.CreateHtmlVersionText(item);

                scope.Dispose();
            }
            // Expected value is 1 for add
            Assert.AreEqual(1, result);
        }

        [Test, Category("Integration")]
        public void UpdateHtmlByVersion_ExpectValidResult()
        {
            // update html for selected version
            int result = 1;
            using (var scope = new TransactionScope())
            {
                var fixture = new Fixture();
                var item = fixture.Create<HtmlText>();
                item.ModuleId = 5;
                //item.VersionNo = ;

                // it will add new version
                this.context.CreateHtmlVersionText(item);

                var itemupdate = fixture.Create<HtmlText>();
                itemupdate.ModuleId = 5;
                itemupdate.VersionNo = 1;

                // it will update version which is added above
                result = this.context.UpdateHtmlVersionText(itemupdate);

                scope.Dispose();
            }
            // Expected value is 2 for updation
            Assert.AreEqual(2, result);
        }

        [Test, Category("Integration")]
        public void PublishHtmlVersion_ExpectValidResut()
        {
            // Publish html based on seleted version
            int result = 1;
            using (var scope = new TransactionScope())
            {
                var fixture = new Fixture();
                var item = fixture.Create<HtmlText>();
                item.ModuleId = 5;
                //item.VersionNo = 1;

                // it will add new version
                this.context.CreateHtmlVersionText(item);

                var itemupdate = fixture.Create<HtmlText>();
                itemupdate.ModuleId = 5;
                itemupdate.VersionNo = 1;
                itemupdate.Published = true;

                // it will add update version which is added with published
                result = this.context.UpdateHtmlVersionText(itemupdate);
                scope.Dispose();
            }
            // Expected value is 2 if updated
            Assert.AreEqual(2, result);
        }

        [Test, Category("Integration")]
        public void GetHtmlVersionHistory_ExpectValidResult()
        {
            List<HtmlText> lstversions = new List<HtmlText>();
            // get all version for html
            using (var scope = new TransactionScope())
            {
                var fixture = new Fixture();
                var item = fixture.Create<HtmlText>();
                item.ModuleId = 5;
                item.VersionNo = 2;

                // it will add new version
                this.context.CreateHtmlVersionText(item);

                lstversions = this.context.GetHtmlVersionList(item.ModuleId);
                scope.Dispose();
            }
            // Expected value is greater than 0 because new version is created
            Assert.Greater(lstversions.Count, 0);
        }
    }
}
