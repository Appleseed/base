namespace Appleseed.Portal.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.SqlClient;
    using System.Linq;

    using Appleseed.Portal.Core;
    using Appleseed.Portal.Core.Models;
    using Appleseed.Portal.Database.Context;

    using Common.Logging;

    public class SecurityDomain
    {
        private readonly ILog log;

        private readonly ISecurityDatabase context;

        public SecurityDomain(ILog log, ISecurityDatabase context)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.log = log;
            this.context = context;
        }

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

            return this.context.ValidateUser(username, password);
        }

        public List<AsUser> GetAllUsers()
        {
            // Check cache for all users

            // if not in cache, get from database

            return this.context.GetAllUsers();
        }

        public string GetUserNameByEmail(string email)
        {
            return this.context.GetUserNameByEmail(email);
        }

        public string GetUserNameByEmail(string portalAlias, string email)
        {
            return this.context.GetUserNameByEmail(portalAlias, email);
        }

        public Membership GetMembershipByUsername(string userName, bool updateLastActivity)
        {
            return this.context.GetMembershipByUsername(userName, updateLastActivity);
        }

        public Membership GetMembershipByUserId(Guid userId, bool updateLastActivity)
        {
            return this.context.GetMembershipByUserId(userId, updateLastActivity);
        }

        public List<AsRole> GetAllRoles()
        {
            return this.context.GetAllRoles();
        }

        public List<ValidationResult> CreateUser(AsUser item)
        {
            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                try
                {
                    this.context.CreateUser(item);
                }
                catch (SqlException dbException)
                {
                    this.log.Debug(dbException);
                    errors.Add(new ValidationResult("A database error has occurred while creating a user.  Error message has been logged."));
                }
                catch (Exception ex)
                {
                    this.log.Debug(ex);
                    errors.Add(new ValidationResult("An unhandled error has occurred while creating a user.  Error message has been logged."));
                }
            }

            return errors;
        }

        public List<ValidationResult> DeleteUser(string portalAlias, string username, bool deleteAllRelatedData)
        {
            var errors = new List<ValidationResult>();
            try
            {
                var result = this.context.DeleteUser(portalAlias, username, true);

                //if (result.Equals(0))
                //{
                //}
            }
            catch (SqlException dbException)
            {
                this.log.Debug(dbException);
                errors.Add(
                    new ValidationResult(
                        "A database error has occurred while deleting a user.  Error message has been logged."));
            }
            catch (Exception ex)
            {
                this.log.Debug(ex);
                errors.Add(
                    new ValidationResult(
                        "An unhandled error has occurred while deleting a user.  Error message has been logged."));
            }

            return errors;
        }

        public List<ValidationResult> UpdateUserEmail(Guid userId, string newEmailAddress)
        {
            var errors = new List<ValidationResult>();
            if (userId.Equals(Guid.Empty))
            {
                throw new ArgumentNullException("userId");
            }

            if (string.IsNullOrEmpty(newEmailAddress))
            {
                throw new ArgumentNullException("newEmailAddress");
            }

            try
            {
                var result = this.context.UpdateUserEmail(userId, newEmailAddress);

                if (result < 1)
                {
                    errors.Add(new ValidationResult("No records were updated."));
                }
            }
            catch (SqlException dbException)
            {
                this.log.Debug(dbException);
                errors.Add(new ValidationResult("A database error has occurred while updating user email.  Error message has been logged."));
            }
            catch (Exception ex)
            {
                this.log.Debug(ex);
                errors.Add(new ValidationResult("An unhandled error has occurred while updating user email.  Error message has been logged."));
            }

            return errors;
        }

        public List<ValidationResult> CreateRole(AsRole item)
        {
            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                try
                {
                    this.context.CreateRole(item);
                }
                catch (SqlException dbException)
                {
                    this.log.Debug(dbException);
                    errors.Add(
                        new ValidationResult(
                            "A database error has occurred while creating role.  Error message has been logged."));
                }
                catch (Exception ex)
                {
                    this.log.Debug(ex);
                    errors.Add(
                        new ValidationResult(
                            "An unhandled error has occurred while creating role.  Error message has been logged."));
                }
            }

            return errors;
        }

        public List<ValidationResult> DeleteRole(Guid roleId, bool deleteOnlyIfRoleIsEmpty)
        {
            var errors = new List<ValidationResult>();
            try
            {
                var result = this.context.DeleteRole(roleId, deleteOnlyIfRoleIsEmpty);

                //if (result.Equals(0))
                //{
                //}
            }
            catch (SqlException dbException)
            {
                this.log.Debug(dbException);
                errors.Add(new ValidationResult("A database error has occurred while deleting a user.  Error message has been logged."));
            }
            catch (Exception ex)
            {
                this.log.Debug(ex);
                errors.Add(new ValidationResult("An unhandled error has occurred while deleting a user.  Error message has been logged."));
            }

            return errors;
        }

        public List<ValidationResult> AddUserToRole(AsRole role, AsUser user)
        {
            var errors = new List<ValidationResult>();
            try
            {
                this.context.AddUserToRole(role, user);
            }
            catch (SqlException dbException)
            {
                this.log.Debug(dbException);
                errors.Add(
                    new ValidationResult(
                        "A database error has occurred while creating a user.  Error message has been logged."));
            }
            catch (Exception ex)
            {
                this.log.Debug(ex);
                errors.Add(
                    new ValidationResult(
                        "An unhandled error has occurred while creating a user.  Error message has been logged."));
            }

            return errors;
        }

        public List<ValidationResult> AddUsersToRoles(List<AsRole> roles, List<AsUser> users)
        {
            var errors = new List<ValidationResult>();
            try
            {
                this.context.AddUsersToRoles(roles, users);
            }
            catch (SqlException dbException)
            {
                this.log.Debug(dbException);
                errors.Add(
                    new ValidationResult(
                        "A database error has occurred while creating a user.  Error message has been logged."));
            }
            catch (Exception ex)
            {
                this.log.Debug(ex);
                errors.Add(
                    new ValidationResult(
                        "An unhandled error has occurred while creating a user.  Error message has been logged."));
            }

            return errors;
        }

        public List<ValidationResult> RemoveUserFromRole(AsRole role, AsUser user)
        {
            var errors = new List<ValidationResult>();
            try
            {
                this.context.RemoveUserFromRole(role, user);
            }
            catch (SqlException dbException)
            {
                this.log.Debug(dbException);
                errors.Add(
                    new ValidationResult(
                        "A database error has occurred while creating a user.  Error message has been logged."));
            }
            catch (Exception ex)
            {
                this.log.Debug(ex);
                errors.Add(
                    new ValidationResult(
                        "An unhandled error has occurred while creating a user.  Error message has been logged."));
            }

            return errors;
        }

        public List<ValidationResult> RemoveUsersFromRoles(List<AsRole> roles, List<AsUser> users)
        {
            var errors = new List<ValidationResult>();
            try
            {
                this.context.RemoveUsersFromRoles(roles, users);
            }
            catch (SqlException dbException)
            {
                this.log.Debug(dbException);
                errors.Add(
                    new ValidationResult(
                        "A database error has occurred while creating a user.  Error message has been logged."));
            }
            catch (Exception ex)
            {
                this.log.Debug(ex);
                errors.Add(
                    new ValidationResult(
                        "An unhandled error has occurred while creating a user.  Error message has been logged."));
            }

            return errors;
        }
    }
}
