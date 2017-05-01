namespace Appleseed.Services.Web.Api
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web.Profile;

    using Appleseed.Portal.Core.Models;
    using Appleseed.Portal.Database.Context;
    using Appleseed.Portal.Domain.Services;

    using Common.Logging;

    using WCFExtras.Soap;

    using SystemSecurity = System.Web.Security;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PortalService : IPortalServices
    {
        private const string ApplicationName = "Appleseed";

        private readonly string connectionString;

        private readonly ILog log;

        private readonly IPortalDomain portalDomain;

        private readonly CmEnvironment environment;

        private readonly string testKey;

        public PortalService()
        {
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] == null)
            {
                throw new NoNullAllowedException("No database connection string found.");
            }

            this.connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            if (string.IsNullOrEmpty(this.connectionString))
            {
                throw new NoNullAllowedException("No database connection string found.");
            }

            this.log = LogManager.GetCurrentClassLogger();

            // Setup Instance Variables
            // Set ASPNET DB Default Application Name to Default Appleseed application
            // Please note this must be changed in order to use Multiportal

            SystemSecurity.Membership.Provider.ApplicationName = ApplicationName;
            ProfileManager.Provider.ApplicationName = ApplicationName;
            SystemSecurity.Roles.ApplicationName = ApplicationName;
            this.testKey = "BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E";

            var env = ConfigurationManager.AppSettings["Environment"];
            if (!string.IsNullOrEmpty(env) && env.ToLower().Equals(CmEnvironment.Production.ToString().ToLower()))
            {
                this.environment = CmEnvironment.Production;
            }
            else
            {
                this.environment = CmEnvironment.Test;
            }

            this.portalDomain = new PortalDomain(this.log, new PortalDatabase(this.log, this.connectionString));
        }

        #region Users
        /// <summary>
        /// Allows a portal user login to be checked.  This metho allows the admin to test that
        /// a connection is working to the Service.  It also allows for logging in users from other sources
        /// using the Appleseed User ASPNETDB as the source of record.  Single Sign On.
        /// </summary>
        /// <returns></returns>
        public bool ValidateUser(int portalId, string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            this.HasPermission();

            var context = new SecurityDomain(this.log, new SecurityDatabase(this.log, this.connectionString, ApplicationName, portalId));
            return context.ValidateUser(username, password);
        }

        public List<AsUser> GetUsers(int portalId)
        {
            this.HasPermission();
            var context = new SecurityDomain(this.log, new SecurityDatabase(this.log, this.connectionString, ApplicationName, portalId));
            var results = context.GetAllUsers();
            return results;
        }

        public List<string> CreateUser(int portalId, AsUser item)
        {
            this.HasPermission();
            var context = new SecurityDomain(this.log, new SecurityDatabase(this.log, this.connectionString, ApplicationName, portalId));

            var result = context.CreateUser(item);
            return (from error in result select error.ErrorMessage).ToList();
        }

        // TODO: Retrieve User

        public List<string> AddUserToRole(int portalId, AsUser user, AsRole role)
        {
            this.HasPermission();
            var context = new SecurityDomain(this.log, new SecurityDatabase(this.log, this.connectionString, ApplicationName, portalId));

            var result = context.AddUserToRole(role, user);
            return (from error in result select error.ErrorMessage).ToList();
        }

        public List<string> RemoveUserFromRole(int portalId, AsUser user, AsRole role)
        {
            this.HasPermission();
            var context = new SecurityDomain(this.log, new SecurityDatabase(this.log, this.connectionString, ApplicationName, portalId));

            var result = context.RemoveUserFromRole(role, user);
            return (from error in result select error.ErrorMessage).ToList();
        }

        public List<string> RemoveUser(int portalId, string username)
        {
            var context = new SecurityDomain(this.log, new SecurityDatabase(this.log, this.connectionString, ApplicationName, portalId));
            var result = context.DeleteUser(ApplicationName, username, true);
            return (from error in result select error.ErrorMessage).ToList();
        }

        public void ChangeUserEmail(int portalId, Guid userId, string newEmailAddress)
        {
            var context = new SecurityDomain(this.log, new SecurityDatabase(this.log, this.connectionString, ApplicationName, portalId));
            context.UpdateUserEmail(userId, newEmailAddress);
        }

        // TODO: Change Password

        public List<string> CreateRole(int portalId, AsRole item)
        {
            this.HasPermission();
            var context = new SecurityDomain(this.log, new SecurityDatabase(this.log, this.connectionString, ApplicationName, portalId));

            var result = context.CreateRole(item);
            return (from error in result select error.ErrorMessage).ToList();
        }

        public List<AsRole> GetRoles(int portalId)
        {
            this.HasPermission();
            var context = new SecurityDomain(this.log, new SecurityDatabase(this.log, this.connectionString, ApplicationName, portalId));
            var results = context.GetAllRoles();
            return results;
        }

        #endregion

        // Done
        #region Pages

        public List<Page> GetPages(int portalId)
        {
            this.HasPermission();
            var moduleDatabase = new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId);
            var moduleDomain = new ModuleDomain(this.log, moduleDatabase);

            return moduleDomain.RetrieveAllPages(portalId);
        }

        public Page GetPage(int portalId, int pageId)
        {
            this.HasPermission();
            var moduleDatabase = new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId);
            var moduleDomain = new ModuleDomain(this.log, moduleDatabase);

            return moduleDomain.RetrievePage(pageId);
        }

        public List<string> CreatePage(int portalId, Page page)
        {
            this.HasPermission();
            var moduleDatabase = new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId);
            var moduleDomain = new ModuleDomain(this.log, moduleDatabase);

            var result = moduleDomain.CreatePage(page);
            return (from error in result select error.ErrorMessage).ToList();
        }

        public List<string> UpdatePage(int portalId, Page page)
        {
            this.HasPermission();
            var moduleDatabase = new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId);
            var moduleDomain = new ModuleDomain(this.log, moduleDatabase);

            var result = moduleDomain.UpdatePage(page);
            return (from error in result select error.ErrorMessage).ToList();
        }

        public List<string> DeletePage(int portalId, int pageId)
        {
            this.HasPermission();
            var moduleDatabase = new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId);
            var moduleDomain = new ModuleDomain(this.log, moduleDatabase);

            var result = moduleDomain.DeletePage(pageId);
            return (from error in result select error.ErrorMessage).ToList();
        }

        #endregion

        // Partially done
        #region PageModules

        /// <summary>
        /// Get a list of all modules for a specific page
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="pageId"></param>
        /// <returns>List of modules on page</returns>
        public List<PageModule> GetModulesOnPage(int portalId, int pageId)
        {
            this.HasPermission();
            var moduleDatabase = new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId);
            var moduleDomain = new ModuleDomain(this.log, moduleDatabase);

            return moduleDomain.RetrieveAllPageModules(pageId);
        }

        public void GetAllInstalledModules()
        {
            throw new NotImplementedException("GetAllInstalledModules has not been implemented.");
        }

        public void UnInstallModule()
        {
            throw new NotImplementedException("UnInstallModule has not been implemented.");
        }

        /// <summary>
        /// Adds a module to a page
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="pageModule"></param>
        /// <returns>List of errors</returns>
        public List<string> AddModuleToPage(int portalId, PageModule pageModule)
        {
            this.HasPermission();
            var moduleDomain = new ModuleDomain(this.log, new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId));

            var result = moduleDomain.CreatePageModule(pageModule);
            return (from error in result select error.ErrorMessage).ToList();
        }

        /// <summary>
        /// Updates a module that is currently installed on a page
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="pageModule"></param>
        /// <returns>List of errors</returns>
        public List<string> ModifyPageModule(int portalId, PageModule pageModule)
        {
            this.HasPermission();
            var moduleDatabase = new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId);
            var moduleDomain = new ModuleDomain(this.log, moduleDatabase);

            var result = moduleDomain.UpdatePageModule(pageModule);
            return (from error in result select error.ErrorMessage).ToList();
        }

        public List<PageModule> GetModuleByPortalID(int portalId)
        {
            this.HasPermission();
            var moduleDatabase = new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId);
            var moduleDomain = new ModuleDomain(this.log, moduleDatabase);
            var result = moduleDomain.GetModulesByPortalID(portalId);
            return result;
        }

        #endregion

        // Need to change 'ChangePortalSettings' method
        #region Portal

        public List<Portal> GetPortals()
        {
            this.HasPermission();

            return this.portalDomain.RetrievePortals();
        }

        public List<string> CreatePortal(Portal portal)
        {
            this.HasPermission();
            var results = this.portalDomain.CreatePortal(portal);

            return (from error in results select error.ErrorMessage).ToList();
        }

        public void ChangePortalSettings()
        {
            throw new NotImplementedException("'ChangePortalSettings' has not been implemented.");
        }

        public List<string> UpdatePortal(Portal portal)
        {
            this.HasPermission();

            var result = this.portalDomain.UpdatePortal(portal);
            return (from error in result select error.ErrorMessage).ToList();
        }

        public List<string> DeletePortal(int portalId)
        {
            this.HasPermission();

            var result = this.portalDomain.DeletePortal(portalId);
            return (from error in result select error.ErrorMessage).ToList();
        }

        #endregion

        #region HtmlText

        public List<string> CreateHtmlText(int portalId, HtmlText item)
        {
            this.HasPermission();
            var moduleDomain = new ModuleDomain(this.log, new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId));

            var result = moduleDomain.CreateHtmlText(item);
            return (from error in result select error.ErrorMessage).ToList();
        }

        public HtmlText RetrieveHtmlText(int portalId, int moduleId)
        {
            this.HasPermission();
            var moduleDomain = new ModuleDomain(this.log, new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId));

            return moduleDomain.RetrieveHtmlText(moduleId);
        }

        public List<string> UpdateHtmlText(int portalId, HtmlText item)
        {
            this.HasPermission();
            var moduleDomain = new ModuleDomain(this.log, new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId));

            var result = moduleDomain.UpdateHtmlText(item);
            return (from error in result select error.ErrorMessage).ToList();
        }

        public List<string> DeleteHtmlText(int portalId, int textId, int versionNo)
        {
            this.HasPermission();
            var moduleDomain = new ModuleDomain(this.log, new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId));

            var result = moduleDomain.DeleteHtmlText(textId, versionNo);
            return (from error in result select error.ErrorMessage).ToList();
        }

        #endregion

        #region HtmlVersion
        /// <summary>
        /// It will create New Html Version, It will returns 1 if addition is successfull
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="item"> HtmlText </param>
        /// <returns> Return value</returns>
        public int CreateNewHtmlVersion(int portalId, HtmlText item)
        {
            this.HasPermission();
            var moduleDomain = new ModuleDomain(this.log, new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId));

            return moduleDomain.CreateHtmlVersionText(item);
        }

        /// <summary>
        /// Update Html, It will returns 2 if updation in successfull
        /// </summary>
        /// <param name="portalId">Portal ID</param>
        /// <param name="item">HtmlText object</param>
        /// <returns> Returns value</returns>
        public int UpdateHtmlByVersion(int portalId, HtmlText item)
        {
            this.HasPermission();
            var moduleDomain = new ModuleDomain(this.log, new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId));

            // check valid version
            if (item.VersionNo > 0)
            {
                var result = moduleDomain.UpdateHtmlVersionText(item);
                return result;
            }
            return -1;
        }

        /// <summary>
        /// Publish Html Version, It will return 2 on successfull publish(update)
        /// </summary>
        /// <param name="portalId">portal id</param>
        /// <param name="item">HtmlText</param>
        /// <returns></returns>
        public int PublishHtmlVersion(int portalId, HtmlText item)
        {
            this.HasPermission();
            var moduleDomain = new ModuleDomain(this.log, new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId));
            item.Published = true;

            return moduleDomain.UpdateHtmlVersionText(item);
        }

        /// <summary>
        /// Get Html By Version No
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <param name="versionNo"> Version No</param>
        /// <param name="moduleId"> Module Id</param>
        /// <returns> return Html Text</returns>
        public HtmlText GetHtmlByVersion(int portalId, int versionNo, int moduleId)
        {
            this.HasPermission();
            var moduleDomain = new ModuleDomain(this.log, new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId));

            return moduleDomain.GetHtmlByVersion(moduleId, versionNo);
        }

        /// <summary>
        /// Get Html Version History
        /// </summary>
        /// <param name="portalId"> Portal Id</param>
        /// <param name="ModuleId"> Module Id</param>
        /// <returns> List of HtmlText</returns>
        public List<HtmlText> GetHtmlVersionHistory(int portalId, int ModuleId)
        {
            this.HasPermission();
            var moduleDomain = new ModuleDomain(this.log, new ModuleDatabase(this.log, this.connectionString, ApplicationName, portalId));

            return moduleDomain.GetHtmlVersionList(ModuleId);
        }

        /// <summary>
        /// Check for valid api details
        /// </summary>
        /// <returns> returns true or false</returns>
        public bool ValidAPIKey()
        {
            this.HasPermission();
            return true;
        }

        #endregion

        /// <summary>
        /// BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E can be used for testing purposes
        /// by adding the below key to web.config/appsettings
        ///  <add key="Environment" value="Test"/>
        /// On Release, A new admin user must be added to the portal to ensure a new unique guid is assigned
        /// </summary>
        /// <returns></returns>
        private void HasPermission()
        {
            var soapHeader = SoapHeaderHelper<AuthHeader>.GetInputHeader("AuthHeader");

            if (soapHeader != null)
            {
                var apiKey = soapHeader.APIKey;
                Guid userGuid;

                if (Guid.TryParse(apiKey, out userGuid))
                {
                    // Environment Check
                    if (apiKey.ToUpper() == this.testKey)
                    {
                        if (this.environment == CmEnvironment.Test)
                        {
                            return;
                        }

                        throw new FaultException("Access Denied : User Key does not have permission.");
                    }

                    // Check to see if this is a valid Appleseed User
                    var user = System.Web.Security.Membership.GetUser(userGuid);
                    if (user != null)
                    {
                        // Check if the user is in the Admins Group
                        if (SystemSecurity.Roles.IsUserInRole(user.UserName, "Admins"))
                        {
                            return;
                        }
                    }
                }
            }

            // Throw Exeception Before going back to method
            throw new FaultException("Access Denied : User Key does not have permission.");
        }



    }
}
