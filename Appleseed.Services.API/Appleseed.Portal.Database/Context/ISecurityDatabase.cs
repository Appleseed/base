namespace Appleseed.Portal.Database.Context
{
    using System;
    using System.Collections.Generic;

    using Appleseed.Portal.Core.Models;

    public interface ISecurityDatabase
    {
        /// <summary>
        /// Allows a portal user login to be checked.  This metho allows the admin to test that
        /// a connection is working to the Service.  It also allows for logging in users from other sources
        /// using the Appleseed User ASPNETDB as the source of record.  Single Sign On.
        /// </summary>
        /// <returns></returns>
        bool ValidateUser(string username, string password);

        /// <summary>
        /// TODO: FINISH THIS!
        /// </summary>
        List<AsUser> GetAllUsers();

        string GetUserNameByEmail(string email);

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
        string GetUserNameByEmail(string portalAlias, string email);

        AsUser RetrieveUser(Guid userId);

        Membership GetMembershipByUsername(string userName, bool updateLastActivity);

        Membership GetMembershipByUserId(Guid userId, bool updateLastActivity);

        /// <summary>
        /// 
        /// </summary>
        List<AsRole> GetAllRoles();

        int CreateUser(AsUser asUser);

        bool DeleteUser(string username, bool deleteAllRelatedData);

        bool DeleteUser(string portalAlias, string username, bool deleteAllRelatedData);

        int UpdateUserEmail(Guid userId, string newEmailAddress);

        int CreateRole(AsRole asRole);

        int DeleteRole(Guid roleId, bool deleteOnlyIfRoleIsEmpty);

        int AddUserToRole(AsRole role, AsUser user);

        int AddUsersToRoles(List<AsRole> roles, List<AsUser> users);

        int RemoveUserFromRole(AsRole role, AsUser user);

        int RemoveUsersFromRoles(List<AsRole> roles, List<AsUser> users);
    }
}