namespace Appleseed.Portal.Database.Context
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    //using System.Web.Security;
    using System.Linq;

    using Appleseed.Portal.Core.Models;
    using Appleseed.Services.Core.Helpers;
    
    using Common.Logging;

    public class SecurityDatabase : BaseDatabase, ISecurityDatabase
    {
        public SecurityDatabase(ILog logger, string connectionString, string applicationName, int portalId)
            : base(logger, connectionString, applicationName, portalId)
        {
        }

        /// <summary>
        /// Allows a portal user login to be checked.  This metho allows the admin to test that
        /// a connection is working to the Service.  It also allows for logging in users from other sources
        /// using the Appleseed User ASPNETDB as the source of record.  Single Sign On.
        /// </summary>
        /// <returns></returns>
        public bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            const string Sproc = "aspnet_Membership_GetPasswordWithFormat";
            var sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ApplicationName", Value = this.ApplicationName };
            sqlParams[1] = new SqlParameter() { ParameterName = "@UserName", Value = username };
            sqlParams[2] = new SqlParameter() { ParameterName = "@UpdateLastLoginActivityDate", Value = 1 };
            sqlParams[3] = new SqlParameter() { ParameterName = "@CurrentTimeUtc", Value = DateTime.UtcNow };
            sqlParams[4] = new SqlParameter() { ParameterName = "@ReturnCode", Direction = ParameterDirection.ReturnValue };

            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    var dbPassword = reader.GetString(0);
                    var dbPasswordSalt = string.Empty;
                    var passwordFormat = System.Web.Security.MembershipPasswordFormat.Clear;

                    passwordFormat = (System.Web.Security.MembershipPasswordFormat)Enum.Parse(typeof(System.Web.Security.MembershipPasswordFormat), reader.GetInt32(1).ToString());
                    dbPasswordSalt = reader.GetString(2);

                    reader.Close();
                    if (((int)sqlParams[4].Value) > 0)
                    {
                        return false;
                    }

                    return this.CheckPassword(password, dbPassword, dbPasswordSalt, passwordFormat);
                }
            }

            return false;
        }

        /// <summary>
        /// TODO: FINISH THIS!
        /// </summary>
        public List<AsUser> GetAllUsers()
        {
            const string Sql = "SELECT u.* FROM [aspnet_Users] u INNER JOIN [aspnet_Applications] a ON a.ApplicationId = u.ApplicationId WHERE LOWER(a.ApplicationName) = LOWER(@ApplicationName);";
            var results = new List<AsUser>();
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ApplicationName", Value = this.ApplicationName };
            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.Text, Sql, sqlParams))
            {
                if (reader.HasRows)
                {
                    var userIdIndex = reader.GetOrdinal("UserID");
                    var applicationIdIndex = reader.GetOrdinal("ApplicationId");
                    var userNameIndex = reader.GetOrdinal("UserName");
                    var loweredUserNameIndex = reader.GetOrdinal("LoweredUserName");
                    var mobileAliasIndex = reader.GetOrdinal("MobileAlias");
                    var isAnonymouseIndex = reader.GetOrdinal("IsAnonymous");
                    var lastActivityDateIndex = reader.GetOrdinal("LastActivityDate");
                    while (reader.Read())
                    {
                        var item = new AsUser()
                            {
                                ApplicationId = reader.GetGuid(applicationIdIndex),
                                IsAnonymous = reader.GetBoolean(isAnonymouseIndex),
                                LastActivityDate = reader.GetDateTime(lastActivityDateIndex),
                                LoweredUserName = reader.GetString(loweredUserNameIndex),
                                MobileAlias = SqlClientHelper.GetNullableString(reader, mobileAliasIndex, null),
                                UserId = reader.GetGuid(userIdIndex),
                                UserName = reader.GetString(userNameIndex)
                            };

                        results.Add(item);
                    }
                }
            }

            return results;
        }

        public string GetUserNameByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            return this.GetUserNameByEmail(this.ApplicationName, email);
        }

        /// <summary>
        /// Takes, as input, an e-mail address and returns the first registered user name whose e-mail address matches the one supplied.
        ///   If it doesn't find a user with a matching e-mail address, GetUserNameByEmail returns an empty string.
        /// </summary>
        /// <param name="portalAlias">
        /// Appleseed's portal alias
        /// </param>
        /// <param name="email">
        /// The email address.
        /// </param>
        /// <returns>
        /// The first registered user name whose e-mail address matches the one supplied.
        ///   If it doesn't find a user with a matching e-mail address, GetUserNameByEmail returns an empty string.
        /// </returns>
        /// <remarks>
        /// </remarks>
        public string GetUserNameByEmail(string portalAlias, string email)
        {
            if (string.IsNullOrEmpty(portalAlias))
            {
                throw new ArgumentNullException("portalAlias");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            } 
            
            const string Sproc = "aspnet_Membership_GetUserByEmail";
            var sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ApplicationName", Value = portalAlias };
            sqlParams[1] = new SqlParameter() { ParameterName = "@Email", Value = email };

            var username = SqlClientHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams);
            return username;
        }

        public AsUser RetrieveUser(Guid userId)
        {
            if (userId.Equals(Guid.Empty))
            {
                throw new ArgumentNullException("userId");
            }

            const string Sql = "SELECT * FROM [aspnet_Users] WHERE [UserId] = @UserId;";
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@UserId", Value = userId };

            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.Text, Sql, sqlParams))
            {
                if (reader.HasRows)
                {
                    var userIdIndex = reader.GetOrdinal("UserID");
                    var applicationIdIndex = reader.GetOrdinal("ApplicationId");
                    var userNameIndex = reader.GetOrdinal("UserName");
                    var loweredUserNameIndex = reader.GetOrdinal("LoweredUserName");
                    var mobileAliasIndex = reader.GetOrdinal("MobileAlias");
                    var isAnonymouseIndex = reader.GetOrdinal("IsAnonymous");
                    var lastActivityDateIndex = reader.GetOrdinal("LastActivityDate");
                    reader.Read();
                    var item = new AsUser()
                    {
                        ApplicationId = reader.GetGuid(applicationIdIndex),
                        IsAnonymous = reader.GetBoolean(isAnonymouseIndex),
                        LastActivityDate = reader.GetDateTime(lastActivityDateIndex),
                        LoweredUserName = reader.GetString(loweredUserNameIndex),
                        MobileAlias = SqlClientHelper.GetNullableString(reader, mobileAliasIndex, null),
                        UserId = reader.GetGuid(userIdIndex),
                        UserName = reader.GetString(userNameIndex)
                    };

                    return item;
                }
            }

            return null;
        }

        public Membership GetMembershipByUsername(string userName, bool updateLastActivity)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }

            const string Sproc = "aspnet_Membership_GetUserByName";
            var sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ApplicationName", Value = this.ApplicationName };
            sqlParams[1] = new SqlParameter() { ParameterName = "@UserName", Value = userName };
            sqlParams[2] = new SqlParameter() { ParameterName = "@CurrentTimeUtc", Value = DateTime.UtcNow };
            sqlParams[3] = new SqlParameter() { ParameterName = "@UpdateLastActivity", Value = updateLastActivity ? 1 : 0 };
            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams))
            {
                if (reader.HasRows)
                {
                    var emailIndex = reader.GetOrdinal("Email");
                    var passwordQuestionIndex = reader.GetOrdinal("PasswordQuestion");
                    var commentIndex = reader.GetOrdinal("Comment");
                    var isApprovedIndex = reader.GetOrdinal("IsApproved");
                    var createDateIndex = reader.GetOrdinal("CreateDate");
                    var lastLoginDateIndex = reader.GetOrdinal("LastLoginDate");
                    var lastPasswordChangedDateIndex = reader.GetOrdinal("LastPasswordChangedDate");
                    var userIdIndex = reader.GetOrdinal("UserId");
                    var isLockedOutIndex = reader.GetOrdinal("IsLockedOut");
                    var lastLockoutDateIndex = reader.GetOrdinal("LastLockoutDate");
                    reader.Read();

                    var item = new Membership()
                    {
                        Email = SqlClientHelper.GetNullableString(reader, emailIndex),
                        PasswordQuestion = SqlClientHelper.GetNullableString(reader, passwordQuestionIndex),
                        Comment = SqlClientHelper.GetNullableString(reader, commentIndex),
                        IsApproved = reader.GetBoolean(isApprovedIndex),
                        CreateDate = reader.GetDateTime(createDateIndex),
                        LastLoginDate = reader.GetDateTime(lastLoginDateIndex),
                        LastPasswordChangedDate = reader.GetDateTime(lastPasswordChangedDateIndex),
                        UserId = reader.GetGuid(userIdIndex),
                        IsLockedOut = reader.GetBoolean(isLockedOutIndex),
                        LastLockoutDate = reader.GetDateTime(lastLockoutDateIndex)
                    };

                    item.AsUser = this.RetrieveUser(item.UserId);
                    return item;
                }
            }

            return null;
        }

        public Membership GetMembershipByUserId(Guid userId, bool updateLastActivity)
        {
            if (userId.Equals(Guid.Empty))
            {
                throw new ArgumentNullException("userId");
            }

            const string Sproc = "aspnet_Membership_GetUserByUserId";
            var sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter() { ParameterName = "@UserId", Value = userId };
            sqlParams[1] = new SqlParameter() { ParameterName = "@CurrentTimeUtc", Value = DateTime.UtcNow };
            sqlParams[2] = new SqlParameter() { ParameterName = "@UpdateLastActivity", Value = updateLastActivity ? 1 : 0 };
            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams))
            {
                if (reader.HasRows)
                {
                    var emailIndex = reader.GetOrdinal("Email");
                    var passwordQuestionIndex = reader.GetOrdinal("PasswordQuestion");
                    var commentIndex = reader.GetOrdinal("Comment");
                    var isApprovedIndex = reader.GetOrdinal("IsApproved");
                    var createDateIndex = reader.GetOrdinal("CreateDate");
                    var lastLoginDateIndex = reader.GetOrdinal("LastLoginDate");
                    var lastPasswordChangedDateIndex = reader.GetOrdinal("LastPasswordChangedDate");
                    var isLockedOutIndex = reader.GetOrdinal("IsLockedOut");
                    var lastLockoutDateIndex = reader.GetOrdinal("LastLockoutDate");
                    reader.Read();

                    var item = new Membership()
                    {
                        Email = SqlClientHelper.GetNullableString(reader, emailIndex),
                        PasswordQuestion = SqlClientHelper.GetNullableString(reader, passwordQuestionIndex),
                        Comment = SqlClientHelper.GetNullableString(reader, commentIndex),
                        IsApproved = reader.GetBoolean(isApprovedIndex),
                        CreateDate = reader.GetDateTime(createDateIndex),
                        LastLoginDate = reader.GetDateTime(lastLoginDateIndex),
                        LastPasswordChangedDate = reader.GetDateTime(lastPasswordChangedDateIndex),
                        UserId = userId,
                        IsLockedOut = reader.GetBoolean(isLockedOutIndex),
                        LastLockoutDate = reader.GetDateTime(lastLockoutDateIndex)
                    };

                    item.AsUser = this.RetrieveUser(item.UserId);
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<AsRole> GetAllRoles()
        {
            const string Sproc = "aspnet_Roles_GetAllRoles";
            var results = new List<AsRole>();
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ApplicationName", Value = this.ApplicationName };
            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams))
            {
                if (reader.HasRows)
                {
                    var roleIdIndex = reader.GetOrdinal("RoleId");
                    var roleNameIndex = reader.GetOrdinal("RoleName");

                    while (reader.Read())
                    {
                        var role = new AsRole()
                                       {
                                           RoleId = reader.GetGuid(roleIdIndex),
                                           RoleName = reader.GetString(roleNameIndex)
                                       };

                        results.Add(role);
                    }
                }
            }

            return results;
        }

        public int CreateUser(AsUser asUser)
        {
            if (asUser == null)
            {
                throw new ArgumentNullException("asUser");
            }

            const string Sproc = "aspnet_Users_CreateUser";

            var sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ApplicationID", Value = asUser.ApplicationId };
            sqlParams[1] = new SqlParameter() { ParameterName = "@UserName", Value = asUser.UserName };
            sqlParams[2] = new SqlParameter() { ParameterName = "@IsUserAnonymous", Value = asUser.IsAnonymous };
            sqlParams[3] = new SqlParameter() { ParameterName = "@LastActivityDate", Value = asUser.LastActivityDate };
            sqlParams[4] = new SqlParameter() { ParameterName = "@UserID", SqlDbType = SqlDbType.UniqueIdentifier, Direction = ParameterDirection.Output };

            var recordsAffected = SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams);
            asUser.UserId = (Guid)sqlParams[4].Value;
            return recordsAffected;
        }

        public bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return this.DeleteUser(this.ApplicationName, username, deleteAllRelatedData);
        }

        public bool DeleteUser(string portalAlias, string username, bool deleteAllRelatedData)
        {
            if (string.IsNullOrEmpty(portalAlias))
            {
                throw new ArgumentNullException("portalAlias");
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username");
            }

            const string Sproc = "aspnet_Users_DeleteUser";

            var sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ApplicationName", Value = this.ApplicationName };
            sqlParams[1] = new SqlParameter() { ParameterName = "@UserName", Value = username };
            sqlParams[2] = new SqlParameter() { ParameterName = "@TablesToDeleteFrom", Value = deleteAllRelatedData ? 0xF : 1 };
            sqlParams[3] = new SqlParameter() { ParameterName = "@NumTablesDeletedFrom", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            sqlParams[4] = new SqlParameter() { ParameterName = "@ReturnCode", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue };

            SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams);

            return (((int)sqlParams[3].Value) > 0) && (((int)sqlParams[4].Value) == 0);
        }

        public int CreateRole(AsRole asRole)
        {
            if (asRole == null)
            {
                throw new ArgumentNullException("asRole");
            }

            const string Sproc = "aspnet_Roles_CreateRole";

            var sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ApplicationName", Value = this.ApplicationName };
            sqlParams[1] = new SqlParameter() { ParameterName = "@RoleName", Value = asRole.RoleName };
            sqlParams[2] = new SqlParameter() { ParameterName = "@NewRoleId", SqlDbType = SqlDbType.UniqueIdentifier, Direction = ParameterDirection.Output };

            var recordsAffected = SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams);
            asRole.RoleId = (Guid)sqlParams[2].Value;
            return recordsAffected;
        }

        public int DeleteRole(Guid roleId, bool deleteOnlyIfRoleIsEmpty)
        {
            if (roleId.Equals(Guid.Empty))
            {
                throw new ArgumentNullException("roleId");
            }

            const string Sproc = "aspnet_Roles_DeleteRole";

            var sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ApplicationName", Value = this.ApplicationName };
            sqlParams[1] = new SqlParameter() { ParameterName = "@RoleId", Value = roleId };
            sqlParams[2] = new SqlParameter() { ParameterName = "@DeleteOnlyIfRoleIsEmpty", Value = deleteOnlyIfRoleIsEmpty,DbType = DbType.Boolean };
            sqlParams[3] = new SqlParameter() { ParameterName = "@ReturnCode", DbType = DbType.Int32,Direction= ParameterDirection.ReturnValue };
            
             SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams);
             return (int)sqlParams[3].Value;
        }

        public int AddUserToRole(AsRole role, AsUser user)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return this.AddUsersToRoles(new List<AsRole>() { role }, new List<AsUser>() { user });
        }

        public int AddUsersToRoles(List<AsRole> roles, List<AsUser> users)
        {
            if (roles == null)
            {
                throw new ArgumentNullException("roles");
            }

            if (users == null)
            {
                throw new ArgumentNullException("users");
            }

            const string Sproc = "aspnet_UsersInRoles_AddUsersToRoles";

            var userList = string.Join(",", (from o in users select o.UserId));
            var roleList = string.Join(",", (from o in roles select o.RoleId));

            var sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ApplicationName", Value = this.ApplicationName.ToLower() };
            sqlParams[1] = new SqlParameter() { ParameterName = "@UserIds", Value = userList };
            sqlParams[2] = new SqlParameter() { ParameterName = "@RoleIds", Value = roleList };

            return SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams);
        }

        public int RemoveUserFromRole(AsRole role, AsUser user)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return this.RemoveUsersFromRoles(new List<AsRole>() { role }, new List<AsUser>() { user });
        }

        public int RemoveUsersFromRoles(List<AsRole> roles, List<AsUser> users)
        {
            if (roles == null)
            {
                throw new ArgumentNullException("roles");
            }

            if (users == null)
            {
                throw new ArgumentNullException("users");
            }

            const string Sproc = "aspnet_UsersInRoles_RemoveUsersFromRoles";

            var userList = string.Join(",", (from o in users select o.UserId));
            var roleList = string.Join(",", (from o in roles select o.RoleId));

            var sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ApplicationName", Value = this.ApplicationName };
            sqlParams[1] = new SqlParameter() { ParameterName = "@UserIds", Value = userList };
            sqlParams[2] = new SqlParameter() { ParameterName = "@RoleIds", Value = roleList };

            return SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams);
        }

        public int UpdateUserEmail(Guid userId, string newEmailAddress)
        {
            if (userId.Equals(Guid.Empty))
            {
                throw new ArgumentNullException("userId");
            }

            if (string.IsNullOrEmpty(newEmailAddress))
            {
                throw new ArgumentNullException("newEmailAddress");
            }

            const string Sql = "UPDATE [aspnet_Membership] SET [Email] = @Email WHERE [UserId] = @UserId;";

            var sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter() { ParameterName = "@Email", Value = newEmailAddress };
            sqlParams[1] = new SqlParameter() { ParameterName = "@UserId", Value = userId };

            return SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, Sql, sqlParams);
        }

        /// <summary>
        /// Compares password values based on the MembershipPasswordFormat.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="dbpassword">
        /// The db password.
        /// </param>
        /// <param name="passwordSalt">
        /// The password Salt.
        /// </param>
        /// <param name="passwordFormat">
        /// The password Format.
        /// </param>
        /// <returns>
        /// The check password.
        /// </returns>
        /// <remarks>
        /// </remarks>
        private bool CheckPassword(string password, string dbpassword, string passwordSalt, System.Web.Security.MembershipPasswordFormat passwordFormat)
        {
            var pass1 = password;
            var pass2 = dbpassword;

            switch (passwordFormat)
            {
                //case MembershipPasswordFormat.Encrypted:
                //    pass1 = this.EncodePassword(dbpassword);
                //    break;
                //case MembershipPasswordFormat.Hashed:
                //    pass1 = this.EncodePassword(passwordSalt + password);
                //    break;
                default:
                    break;
            }

            return pass1.Equals(pass2);
        }
    }
}
