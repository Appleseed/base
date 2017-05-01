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
    public class SecurityDatabaseTests
    {
        private ILog log;

        private SecurityDatabase database;

        [SetUp]
        public void Setup()
        {
            this.log = new Mock<ILog>().Object;
            this.database = new SecurityDatabase(this.log, ConfigurationManager.ConnectionStrings["AppleseedConnectionString"].ConnectionString, "Appleseed", 0);
        }

        [Test, Category("Integration")]
        public void ValidateUser_ReturnsTrue()
        {
            // Arrange
            var email = "admin@Appleseedportal.net";
            var password = "admin";

            // Act
            var result = this.database.ValidateUser(email, password);

            // Assert
            Assert.IsTrue(result);
        }

        [Test, Category("Integration")]
        public void ValidateUser_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var email = "admin@Appleseedportal.net";
            var password = "asdfasdf";

            // Act
            var result = this.database.ValidateUser(email, password);

            // Assert
            Assert.IsFalse(result);
        }

        [Test, Category("Integration")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ValidateUser_BlankPassword_Fails()
        {
            // Arrange
            var email = "admin@Appleseedportal.net";
            var password = "";

            // Act
            this.database.ValidateUser(email, password);

            // Assert
        }

        [Test, Category("Integration")]
        public void GetAllUsers_ReturnsList()
        {
            // Arrange
            List<AsUser> result;

            // Act
            result = this.database.GetAllUsers();

            // Assert
            Assert.IsTrue(result.Count > 0);
        }

        [Test, Category("Integration")]
        public void GetUserNameByEmail_ReturnsValidUsername()
        {
            // Arrange
            var expected = "admin@Appleseedportal.net";
            var email = "admin@Appleseedportal.net";

            // Act
            var result = this.database.GetUserNameByEmail(email);

            // Assert
            Assert.IsNotNullOrEmpty(result);
            Assert.AreEqual(expected, result);
        }

        [Test, Category("Integration")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void GetUserNameByEmail_BlankEmail_Fails()
        {
            // Arrange
            var invalidEmail = "";

            // Act
            this.database.GetUserNameByEmail(invalidEmail);

            // Assert
        }

        [Test, Category("Integration")]
        public void GetUserNameByEmail_InvalidEmail_Fails()
        {
            // Arrange
            var invalidEmail = "asdf@asdf.com";

            // Act
            var result = this.database.GetUserNameByEmail(invalidEmail);

            // Assert
            Assert.IsNullOrEmpty(result);
        }
        
        [Test, Category("Integration")]
        public void RetrieveUser_ValidId_ReturnsValidUser()
        {
            // Arrange
            var userId = new Guid("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E");

            // Act
            var result = this.database.RetrieveUser(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
        }

        [Test, Category("Integration")]
        public void RetrieveUser_InalidId_ReturnsNull()
        {
            // Arrange
            var userId = new Guid("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7F");

            // Act
            var result = this.database.RetrieveUser(userId);

            // Assert
            Assert.IsNull(result);
        }

        [Test, Category("Integration")]
        public void GetMembershipByUserId_ReturnsValidMembership()
        {
            // Arrange
            var userId = new Guid("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E");

            // Act
            var result = this.database.GetMembershipByUserId(userId, true);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.AsUser.UserId);
        }

        [Test, Category("Integration")]
        public void GetAllRoles_ReturnsValidRoles()
        {
            // Arrange

            // Act
            var result = this.database.GetAllRoles();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [Test, Category("Integration")]
        public void CreateUser_ReturnsSuccess()
        {
            // Arrange
            var fixture = new Fixture();
            var item = fixture.Create<AsUser>();
            item.ApplicationId = new Guid("CA8DEE6B-0B81-4462-AF3B-6A22B36A0304");
            var expected = 1;
            int affectedRecordCount = 0;
            
            // Act
            using (var scope = new TransactionScope())
            {
                affectedRecordCount = this.database.CreateUser(item);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.AreEqual(expected, affectedRecordCount);
        }

        [Test, Category("Integration")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void CreateUser_ThrowsError()
        {
            // Arrange

            // Act
            this.database.CreateUser(null);

            // Assert
        }

        [Test, Category("Integration")]
        public void DeleteUser_ReturnsSuccess()
        {
            // Arrange
            var email = "admin@Appleseedportal.net";
            bool result;

            // Act
            using (var scope = new TransactionScope())
            {
                result = this.database.DeleteUser(email, true);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.IsTrue(result);
        }

        [Test, Category("Integration")]
        public void DeleteUser_InvalidEmail_Fails()
        {
            // Arrange
            var email = "bob@Appleseedportal.net";
            bool result;

            // Act
            using (var scope = new TransactionScope())
            {
                result = this.database.DeleteUser(email, true);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.IsFalse(result);
        }

        [Test, Category("Integration")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void DeleteUser_ThrowsError()
        {
            // Arrange

            // Act
            this.database.DeleteUser(string.Empty, true);

            // Assert
        }

        [Test, Category("Integration")]
        public void CreateRole_ReturnsSuccess()
        {
            // Arrange
            var fixture = new Fixture();
            var item = fixture.Create<AsRole>();
            item.ApplicationId = new Guid("CA8DEE6B-0B81-4462-AF3B-6A22B36A0304");
            var expected = 1;
            int affectedRecordCount = 0;

            // Act
            using (var scope = new TransactionScope())
            {
                affectedRecordCount = this.database.CreateRole(item);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.AreEqual(expected, affectedRecordCount);
        }

        [Test, Category("Integration")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void CreateRole_ThrowsError()
        {
            // Arrange

            // Act
            this.database.CreateRole(null);

            // Assert
        }

        [Test, Category("Integration")]
       // [Ignore("Currently failing, looks like there may be an issue with the 'Transaction' inside the sproc.")]
        public void DeleteRole_ReturnsSuccess()
        {
            // Arrange
            var role = new Guid("F6A4ADDA-8450-4F9A-BE86-D0719B239A8D");
            int result;
            var fixture = new Fixture();
            var item = fixture.Create<AsRole>();
            item.ApplicationId = new Guid("CA8DEE6B-0B81-4462-AF3B-6A22B36A0304");

            // Act
            using (var scope = new TransactionScope())
            {
                int addrole = this.database.CreateRole(item);

                result = this.database.DeleteRole(item.RoleId, true);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            // Expected value is zero because sp return 0 in case of successful deleting a role
            Assert.AreEqual(0, result);
        }

        [Test, Category("Integration")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void DeleteRole_InvalidEmail_Fails()
        {
            // Arrange
            
            // Act
            this.database.DeleteRole(new Guid(), true);

            // Assert
        }

        [Test, Category("Integration")]
        public void AddUserToRole_ValidUserValidRole_Success()
        {
            // Arrange
            int result;
            // 2 rows get affected after addition from sp
            const int Expected = 2;
            var user = new AsUser() { ApplicationId = Guid.Parse("CA8DEE6B-0B81-4462-AF3B-6A22B36A0304"), UserId = Guid.Parse("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E") };
            var role = new AsRole() { ApplicationId = Guid.Parse("CA8DEE6B-0B81-4462-AF3B-6A22B36A0304"), RoleId = Guid.Parse("F6A4ADDA-8450-4F9A-BE86-D0719B239A8D") };

            // Act
            using (var scope = new TransactionScope())
            {
                result = this.database.AddUserToRole(role, user);
            
                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.AreEqual(Expected, result);
        }

        [Test, Category("Integration")]
        public void AddUserToRole_ValidUserInvalidRole_Fail()
        {
            // Arrange
            int result;
            const int Expected = -1;
            var user = new AsUser() { UserId = Guid.Parse("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E") };
            var role = new AsRole() { RoleId = Guid.Parse("F6A4ADDA-8450-4F9A-BE86-D0719B239A8A") };

            // Act
            using (var scope = new TransactionScope())
            {
                result = this.database.AddUserToRole(role, user);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.AreEqual(Expected, result);
        }

        [Test, Category("Integration")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void AddUserToRole_ValidUserNullRole_Fails()
        {
            // Arrange
            var user = new AsUser() { UserId = Guid.Parse("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E") };

            // Act
            this.database.AddUserToRole(null, user);

            // Assert
        }

        [Test, Category("Integration")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void AddUserToRole_NullUserValidRole_Fails()
        {
            // Arrange
            var role = new AsRole() { RoleId = Guid.Parse("F6A4ADDA-8450-4F9A-BE86-D0719B239A8D") };

            // Act
            this.database.AddUserToRole(role, null);

            // Assert
        }


        [Test, Category("Integration")]
        public void RemoveUserFromRole_ValidUserValidRole_Success()
        {
            // Arrange
            int result;
            // 3 rows get affected from the sp
            const int Expected = 3;
            var user = new AsUser() { UserId = Guid.Parse("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E") };
            var role = new AsRole() { RoleId = Guid.Parse("F6A4ADDA-8450-4F9A-BE86-D0719B239A8D") };

            // Act
            using (var scope = new TransactionScope())
            {
                result = this.database.RemoveUserFromRole(role, user);

                // Calling scope.Dispose() prevents the transaction from commiting the data to the database.
                scope.Dispose();
            }

            // Assert
            Assert.AreEqual(Expected, result);
        }
    }
}
